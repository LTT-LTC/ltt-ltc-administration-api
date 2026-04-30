using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Screens.Dtos.Input;
using LTC.AdministrationService.Admin.Screens.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Admin.Screens
{
    public interface IAdminScreenAppService : IApplicationService
    {
        Task<PagedResultDto<ScreenOutputDto>> GetScreenListAsync(Guid cinemaId, GetScreenListInputDto input);
        Task<ScreenOutputDto> GetScreenAsync(Guid id);
        Task<ScreenOutputDto> CreateScreenAsync(Guid cinemaId, CreateScreenInputDto input);
        Task<ScreenOutputDto> UpdateScreenAsync(Guid id, UpdateScreenInputDto input);
        Task DeleteScreenAsync(Guid id);
    }
}
