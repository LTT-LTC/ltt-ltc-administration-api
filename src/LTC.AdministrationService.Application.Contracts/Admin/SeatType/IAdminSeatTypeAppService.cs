using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.SeatType.Dtos.Input;
using LTC.AdministrationService.Admin.SeatType.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Admin.SeatType
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