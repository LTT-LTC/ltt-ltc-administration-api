using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Admin.Cinemas
{
    public interface IAdminCinemaAppService : IApplicationService
    {
        Task<PagedResultDto<CinemasOutputDto>> GetCinemaListAsync(GetCinemasListInputDto input);
        Task<CinemasOutputDto> GetCinemaAsync(Guid id);
        Task<CinemasOutputDto> CreateCinemaAsync(CreateCinemasInputDto input);
        Task<CinemasOutputDto> UpdateCinemaAsync(Guid id, UpdateCinemasInputDto input);
        Task DeleteCinemaAsync(Guid id);
    }
}
