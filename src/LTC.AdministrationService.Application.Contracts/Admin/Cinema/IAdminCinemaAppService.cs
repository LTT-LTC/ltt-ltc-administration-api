using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Cinema.Dtos.Input;
using LTC.AdministrationService.Admin.Cinema.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Admin.Cinema
{
    public interface IAdminCinemaAppService : IApplicationService
    {
        Task<PagedResultDto<CinemaOutputDto>> GetListAsync(GetCinemaListInputDto input);
        Task<CinemaOutputDto> GetAsync(Guid id);
        Task<CinemaOutputDto> CreateAsync(CreateCinemaInputDto input);
        Task<CinemaOutputDto> UpdateAsync(Guid id, UpdateCinemaInputDto input);
        Task DeleteAsync(Guid id);
    }
}
