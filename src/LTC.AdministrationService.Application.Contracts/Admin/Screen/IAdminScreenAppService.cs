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
        Task<PagedResultDto<ScreenOutputDto>> GetListAsync(Guid cinemaId, GetScreenListInputDto input);
        Task<ScreenOutputDto> GetAsync(Guid id);
        Task<ScreenOutputDto> CreateAsync(Guid cinemaId, CreateScreenInputDto input);
        Task<ScreenOutputDto> UpdateAsync(Guid id, UpdateScreenInputDto input);
        Task DeleteAsync(Guid id);
    }
}
