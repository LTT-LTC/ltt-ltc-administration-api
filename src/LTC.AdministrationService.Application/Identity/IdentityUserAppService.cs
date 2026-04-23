using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Identity.Dtos.Input;
using LTC.AdministrationService.Identity.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Uow;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace LTC.AdministrationService.Identity
{
    public class IdentityUserAppService(
        IRepository<IdentityUser, Guid> identityUserRepository,
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<IdentityOptions> identityOptions,
        IConfiguration configuration,
        IdentityUserManager _userManager
        ) : AdministrationServiceAppService, IIdentityUserAppService
    {

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Guid> CreateAsync(CreateUserInputDto input)
        {
            var passwordSettings = configuration.GetSection("PasswordSettings").Get<PasswordSettingsDto>();
            await identityOptions.SetAsync();
            var user = new IdentityUser(Guid.CreateVersion7(), input.Email, input.Email, CurrentTenant.Id);
            user.Name = input.Name;
            input.MapExtraPropertiesTo(user);
            string password = passwordSettings.IsEnabled ? passwordSettings.DefaultPassword : NumericExtensions.RandomNumber().ToString();
            user.SetEmailConfirmed(true);
            if (!string.IsNullOrEmpty(input.PhoneNumber))
            {
                user.SetPhoneNumber(input.PhoneNumber, true);
            }
            user.SetIsActive(input.IsActive);
            (await _userManager.CreateAsync(user, password)).CheckErrors();

            if (input.Roles != null && input.Roles.Count > 0)
            {
                (await _userManager.SetRolesAsync(user, input.Roles)).CheckErrors();
            }

            return user.Id;
        }
    }
}
