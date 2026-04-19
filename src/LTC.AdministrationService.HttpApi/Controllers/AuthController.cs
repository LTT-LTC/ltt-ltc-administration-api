using LTC.AdministrationService.Auth;
using LTC.AdministrationService.Auth.Dtos.Input;
using LTC.Shared.Hosting.Microservices.HttpApi;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Controllers
{
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/auth")]
    public class AuthController(
        IAuthAppService authAppService,
        IAntiforgery antiforgery
        ) : AppControllerBase
    {
        [HttpGet("antiforgery-token")]
        [AllowAnonymous]
        public IActionResult GetAntiforgeryToken()
        {
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new
            {
                token = tokens.RequestToken,
                headerName = tokens.HeaderName
            });
        }

        [HttpPost("request-password-recovery")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordRecoveryAsync(RequestPasswordRecoveryInputDto input)
        => Success(await authAppService.RequestPasswordRecoveryAsync(input));

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordInputDto input)
            => Success(await authAppService.ResetPasswordAsync(input));

        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginInputDto input)
            => Success(await authAppService.LoginAsync(input));

        [HttpPost("refresh-login")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshLoginAsync(RefreshLoginInputDto input)
            => Success(await authAppService.RefreshLoginAsync(input));

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(LogoutInputDto input)
            => Success(await authAppService.LogoutAsync(input));
    }
}
