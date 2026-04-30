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
        Task<PagedResultDto<SeatTypeOutputDto>> GetSeatTypeListAsync(GetSeatTypeListInputDto input);
        Task<SeatTypeOutputDto> GetSeatTypeAsync(Guid id);
        Task<SeatTypeOutputDto> CreateSeatTypeAsync(CreateSeatTypeInputDto input);
        Task<SeatTypeOutputDto> UpdateSeatTypeAsync(Guid id, UpdateSeatTypeInputDto input);
        Task DeleteSeatTypeAsync(Guid id);
    }
}