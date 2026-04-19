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
        Task<PagedResultDto<CinemasOutputDto>> GetListAsync(GetCinemasListInputDto input);
        Task<CinemasOutputDto> GetAsync(Guid id);
        Task<CinemasOutputDto> CreateAsync(CreateCinemasInputDto input);
        Task<CinemasOutputDto> UpdateAsync(Guid id, UpdateCinemasInputDto input);
        Task DeleteAsync(Guid id);
    }
}
