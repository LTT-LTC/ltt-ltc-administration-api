using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Input;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Admin.SeatTypes
{
    public interface IAdminSeatTypeAppService : IApplicationService
    {
        Task<PagedResultDto<SeatTypeOutputDto>> GetListAsync(GetSeatTypeListInputDto input);
        Task<SeatTypeOutputDto> GetAsync(Guid id);
        Task<SeatTypeOutputDto> CreateAsync(CreateSeatTypeInputDto input);
        Task<SeatTypeOutputDto> UpdateAsync(Guid id, UpdateSeatTypeInputDto input);
        Task DeleteAsync(Guid id);
    }
}