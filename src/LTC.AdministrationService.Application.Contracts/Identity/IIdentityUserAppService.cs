using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Identity.Dtos.Input;
using System;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace LTC.AdministrationService.Identity
{
    public interface IIdentityUserAppService
    {
        Task<Guid> CreateAsync(CreateUserInputDto input);
    }
}
