using LTC.AdministrationService.Auth.Dtos.Input;
using LTC.AdministrationService.Auth.Dtos.Output;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Auth
{
    public interface IAuthAppService
    {
        Task<bool> RequestPasswordRecoveryAsync(RequestPasswordRecoveryInputDto input);
        Task<bool> ResetPasswordAsync(ResetPasswordInputDto input);
        Task<LoginOutputDto> LoginAsync(LoginInputDto input);
        Task<LoginOutputDto> RefreshLoginAsync(RefreshLoginInputDto input);
        Task<bool> LogoutAsync(LogoutInputDto input);
    }
}
