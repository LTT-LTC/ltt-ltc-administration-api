using LTC.AdministrationService.Auth.Dtos.Input;
using LTC.AdministrationService.Auth.Dtos.Output;
using LTC.AdministrationService.Entities.CacheKeys;
using LTC.AdministrationService.Events;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using LTC.Shared.Hosting.Microservices.Authentication;
using LTC.Shared.Hosting.Microservices.Timing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace LTC.AdministrationService.Auth
{
    public class AuthAppService : AdministrationServiceAppService, IAuthAppService
    {
        private readonly IdentityUserManager _identityUserManager;
        private readonly AbpSignInManager _signInManager;
        private readonly TokenAuthOption _tokenAuthOption;
        private readonly IDistributedCache<string, UserRefreshTokenDto> _cacheToken;
        private readonly IDistributedCache<string, string> _cacheIpAddress;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILocalEventBus _localEventBus;
        private readonly IDistributedCache<PasswordResetTokenCacheItem, PasswordResetTokenCacheKey> _passwordResetTokenCache;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entities.Employee> _employeeRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConnectionMultiplexer _redis;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IDataFilter _dataFilter;
        private readonly IGmt7Clock _gmt7Clock;

        public AuthAppService(
            IIdentityUserRepository identityUserRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IdentityUserManager identityUserManager,
            AbpSignInManager signInManager,
            ILocalEventBus localEventBus,
            IConfiguration configuration,
            IDistributedCache<string, UserRefreshTokenDto> cacheToken,
            IOptions<TokenAuthOption> tokenAuthOption,
            IRepository<Entities.Employee> employeeRepository,
            ICurrentTenant currentTenant,
            IDistributedCache<PasswordResetTokenCacheItem, PasswordResetTokenCacheKey> passwordResetTokenCache,
            IDistributedCache<string, string> cacheIpAddress,
            IHttpContextAccessor httpContextAccessor,
            IConnectionMultiplexer redis,
            IRepository<IdentityUser, Guid> userRepository,
            ICurrentUser currentUser,
            IDataFilter dataFilter,
            IGmt7Clock gmt7Clock
            )
        {
            _identityUserRepository = identityUserRepository;
            _identityUserManager = identityUserManager;
            _signInManager = signInManager;
            _unitOfWorkManager = unitOfWorkManager;
            _localEventBus = localEventBus;
            _passwordResetTokenCache = passwordResetTokenCache;
            _configuration = configuration;
            _cacheToken = cacheToken;
            _tokenAuthOption = tokenAuthOption.Value;
            _employeeRepository = employeeRepository;
            _currentTenant = currentTenant;
            _httpContextAccessor = httpContextAccessor;
            _cacheIpAddress = cacheIpAddress;
            _redis = redis;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _dataFilter = dataFilter;
            _gmt7Clock = gmt7Clock;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<LoginOutputDto> LoginAsync(LoginInputDto input)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var loginIdentifier = input.UserName?.Trim();
                if (string.IsNullOrWhiteSpace(loginIdentifier))
                {
                    throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["InvalidValuePlsReEnter"], L["AccountOrPassword"]));
                }

                var employeeQueryable = await _employeeRepository.GetQueryableAsync();
                var employee = await employeeQueryable.FirstOrDefaultAsync(x => x.EmployeeId == loginIdentifier);

                IdentityUser? identityUser = null;
                if (employee?.UserId.HasValue == true)
                {
                    identityUser = await _identityUserManager.FindByIdAsync(employee.UserId.Value.ToString());
                }

                identityUser ??= await _identityUserManager.FindByNameAsync(loginIdentifier);
                identityUser ??= await _identityUserManager.FindByEmailAsync(loginIdentifier);
                identityUser ??= await FindUserAcrossTenantsAsync(loginIdentifier);

                if (identityUser == null)
                {
                    throw new UserFriendlyException(L["UserNotFound"]);
                }

                employee ??= await employeeQueryable.FirstOrDefaultAsync(x => x.UserId == identityUser.Id);

                var authorizationResult = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, true);
                if (!authorizationResult.Succeeded)
                {
                    if (!authorizationResult.IsNotAllowed)
                    {
                        await uow.CompleteAsync();
                    }
                    if (authorizationResult.IsLockedOut || !identityUser.IsActive)
                    {
                        //await BlockIpAddressAsync();
                        throw new UserFriendlyException(L["AccountIsBanned"]);
                    }
                    //await BlockIpAddressAsync();
                    throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["InvalidValuePlsReEnter"], L["AccountOrPassword"]));
                }

                if (identityUser.TenantId != _currentTenant.Id)
                {
                    throw new UserFriendlyException(L["InvalidTenantForUser"]);
                }

                var loginResult = await CreateAccessTokenAsync(identityUser);

                // kiểm tra lần đầu đăng nhập
                if (employee?.IsFirstLogin == true)
                {
                    return new LoginOutputDto
                    {
                        AccessToken = loginResult.AccessToken,
                        RefreshToken = loginResult.RefreshToken,
                        UserName = identityUser.UserName,
                        IsOTPSent = false,
                        IsFirstLogin = true
                    };
                }

                await uow.CompleteAsync();
                return loginResult;
            }
        }

        private async Task<IdentityUser?> FindUserAcrossTenantsAsync(string loginIdentifier)
        {
            var normalizedLogin = loginIdentifier.Trim().ToUpperInvariant();

            using (_dataFilter.Disable<IMultiTenant>())
            {
                var usersQueryable = await _userRepository.GetQueryableAsync();

                return await usersQueryable.FirstOrDefaultAsync(x =>
                    x.NormalizedUserName == normalizedLogin ||
                    x.NormalizedEmail == normalizedLogin ||
                    x.UserName == loginIdentifier ||
                    x.Email == loginIdentifier);
            }
        }

        /// <summary>
        /// block ip address nếu có quá nhiều request đăng nhập thất bại từ ip đó trong một khoảng thời gian ngắn để tránh bị tấn công brute-force
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task BlockIpAddressAsync()
        {
            var clientIPAddress = GetClientIpAddress();
            var db = _redis.GetDatabase();
            var ipKey = $"{_currentTenant.Id}:login-ip:{clientIPAddress}";

            var ipCount = await db.StringIncrementAsync(ipKey);

            if (ipCount == 1)
            {
                await db.KeyExpireAsync(ipKey, TimeSpan.FromMinutes(30));
            }

            if (ipCount > 3)
            {
                var blockKey = $"{_currentTenant.Id}:blocked-ip:{clientIPAddress}";
                await db.StringSetAsync(blockKey, "1", TimeSpan.FromMinutes(5));

                throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["TooManyRequest"], $"5 {L["Minutes"]}"));
            }
        }

        private async Task<LoginOutputDto> SendOTPMail(IdentityUser user)
        {
            // gửi OTP
            await _localEventBus.PublishAsync(new SendOTPEvent
            {
                UserId = user.Id,
                ExpireSeconds = AuthConsts.OTP_EXPIRE_SECONDS,
            });
            return new LoginOutputDto
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                UserName = user.UserName,
                IsOTPSent = true
            };
        }

        /// <summary>
        /// Request password recovery
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> RequestPasswordRecoveryAsync(RequestPasswordRecoveryInputDto input)
        {
            var employeeQueryable = await _employeeRepository.GetQueryableAsync();
            var employee = await employeeQueryable.Where(x => x.EmployeeId == input.UserName.Trim()).FirstOrDefaultAsync()
                ?? throw new UserFriendlyException(L["UserNotFound"]);

            var user = await _identityUserManager.FindByIdAsync(employee.UserId.Value.ToString())
                ?? throw new UserFriendlyException(L["UserNotFound"]);

            // Generate unique token
            var token = Guid.NewGuid().ToString();

            // Store in Redis with configurable expiration
            var cacheItem = new PasswordResetTokenCacheItem
            {
                UserId = user.Id,
                Token = token,
                Email = user.Email,
                ExpirationTime = _gmt7Clock.Gmt7Now.AddHours(AuthConsts.PASSWORD_RESET_TOKEN_EXPIRE_HOURS),
                IsUsed = false
            };

            await _passwordResetTokenCache.SetAsync(
                new PasswordResetTokenCacheKey { Token = token },
                cacheItem,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(AuthConsts.PASSWORD_RESET_TOKEN_EXPIRE_HOURS)
                }
            );

            var frontendUrl = _configuration["App:ResetPasswordUrl"];
            var resetUrl = $"{frontendUrl}?token={token}";

            // Publish event to send email
            await _localEventBus.PublishAsync(new RecoveryPasswordEvent
            {
                Email = user.Email,
                Name = user.Name,
                UserName = user.UserName,
                ResetToken = token,
                ResetUrl = resetUrl,
                ExpireHours = AuthConsts.PASSWORD_RESET_TOKEN_EXPIRE_HOURS
            });

            return true;
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> ResetPasswordAsync(ResetPasswordInputDto input)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // Get token from Redis
                var cacheKey = new PasswordResetTokenCacheKey { Token = input.Token };
                var resetToken = await _passwordResetTokenCache.GetAsync(cacheKey);

                if (resetToken == null)
                    throw new UserFriendlyException(L["InvalidToken"]);

                if (resetToken.IsUsed)
                    throw new UserFriendlyException(L["TokenAlreadyUsed"]);

                if (resetToken.ExpirationTime < _gmt7Clock.Gmt7Now)
                    throw new UserFriendlyException(L["TokenExpired"]);

                // Find user
                var user = await _identityUserManager.FindByEmailAsync(resetToken.Email);
                if (user == null)
                    throw new UserFriendlyException(L["UserNotFound"]);

                // Check if new password is same as old password
                var isSamePassword = await _identityUserManager.CheckPasswordAsync(user, input.Password);
                if (isSamePassword)
                    throw new UserFriendlyException(L["PasswordCannotBeSameAsOld"]);

                // Reset password
                await _identityUserManager.RemovePasswordAsync(user);
                var result = await _identityUserManager.AddPasswordAsync(user, input.Password);

                if (!result.Succeeded)
                    throw new UserFriendlyException(L["PasswordResetFailed"]);

                // Mark token as used
                resetToken.IsUsed = true;
                await _passwordResetTokenCache.SetAsync(
                    cacheKey,
                    resetToken,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(AuthConsts.PASSWORD_RESET_TOKEN_EXPIRE_HOURS)
                    }
                );

                // Publish success event to send confirmation email
                var now = _gmt7Clock.Gmt7Now;
                await _localEventBus.PublishAsync(new PasswordResetSuccessEvent
                {
                    Email = user.Email,
                    Name = user.Name ?? user.UserName,
                    UserName = user.UserName,
                    Time = now.ToString("HH:mm:ss"),
                    Date = now.ToString("dd/MM/yyyy")
                });

                await uow.CompleteAsync();
                return true;
            }
        }

        /// <summary>
        /// Tạo access token, refresh token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<LoginOutputDto> CreateAccessTokenAsync(IdentityUser user)
        {
            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var claims = claimsPrincipal.Claims.ToList();

            IList<string> roles;
            using (_dataFilter.Disable<IMultiTenant>())
            {
                roles = await _identityUserManager.GetRolesAsync(user);
            }
            
            foreach (var role in roles)
            {
                if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            string sessionId = Guid.CreateVersion7().ToString();
            claims.Add(new Claim("sessionId", sessionId));

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenAuthOption.Issuer,
                audience: _tokenAuthOption.Audience,
                claims: claims,
                notBefore: _gmt7Clock.UtcNow,
                expires: _gmt7Clock.UtcNow.AddHours(_tokenAuthOption.Expiration),
                signingCredentials: _tokenAuthOption.SigningCredentials
            );

            var refreshToken = $"{Guid.CreateVersion7().ToString()}-{Guid.CreateVersion7().ToString()}";
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            await _cacheToken.SetAsync(new UserRefreshTokenDto { SessionId = sessionId, UserId = $"{user.Id}" }, refreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(_tokenAuthOption.RefreshExpiration)
            });

            return new LoginOutputDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = user.UserName,
            };
        }

        /// <summary>
        /// Refresh Login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<LoginOutputDto> RefreshLoginAsync(RefreshLoginInputDto input)
        {
            var userId = GetUserIdFromAccessToken(input.AccessToken);
            var sessionId = GetSessionIdFromAccessToken(input.AccessToken);
            var userRefreshToken = await _cacheToken.GetAsync(new UserRefreshTokenDto { SessionId = sessionId, UserId = userId });
            if (userRefreshToken == null)
            {
                throw new UserFriendlyException(L["InvalidRefreshToken"]);
            }
            await _cacheToken.RemoveAsync(new UserRefreshTokenDto { SessionId = sessionId, UserId = userId });

            var user = await _identityUserRepository.FindAsync(Guid.Parse(userId))
                ?? throw new UserFriendlyException(L["UserNotFound"]); ;

            if (!user.IsActive)
                throw new UserFriendlyException(L["AccountIsBanned"]);

            if (user.TenantId != _currentTenant.Id)
            {
                throw new UserFriendlyException(L["InvalidTenantForUser"]);
            }

            var loginResult = await CreateAccessTokenAsync(user);
            return loginResult;
        }

        /// <summary>
        /// Lấy User Id trong access token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private string GetUserIdFromAccessToken(string token)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                string userId = jwt?.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return userId;
            }
            catch
            {
                throw new UserFriendlyException("Phiên đăng nhập đã hết hạn hoặc không hợp lệ!");
            }
        }

        /// <summary>
        /// Lấy Session Id trong access token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        private string GetSessionIdFromAccessToken(string token)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                string sessionId = jwt?.Claims.First(c => c.Type == "sessionId")?.Value;
                return sessionId;
            }
            catch
            {
                throw new UserFriendlyException("Phiên đăng nhập đã hết hạn hoặc không hợp lệ!");
            }
        }

        /// <summary>
        /// lấy ip address của client
        /// </summary>
        /// <returns></returns>
        private string GetClientIpAddress()
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return ipAddress ?? "";
        }

        /// <summary>
        /// Đăng xuất, xóa refresh token trong cache để token không thể sử dụng để refresh access token nữa
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> LogoutAsync(LogoutInputDto input)
        {
            var sessionId = CurrentUser.FindClaim("sessionId");
            var userId = _currentUser.Id?.ToString();
            await _cacheToken.RemoveAsync(new UserRefreshTokenDto { SessionId = sessionId?.Value, UserId = userId });
            return true;
        }
    }
}
