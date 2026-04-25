using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Admin.SeatMaps.Dtos.Input;
using LTC.AdministrationService.Admin.SeatMaps.Dtos.Output;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Admin.SeatMaps
{
    public interface IAdminSeatMapAppService : IApplicationService
    {
        Task<PagedResultDto<SeatMapOutputDto>> GetListAsync(Guid cinemaId, GetSeatMapListInputDto input);
        Task<SeatMapOutputDto> GetAsync(Guid id);
        Task<SeatMapOutputDto> CreateAsync(Guid cinemaId, CreateSeatMapInputDto input);
        Task<SeatMapOutputDto> UpdateAsync(Guid id, UpdateSeatMapInputDto input);
        Task DeleteAsync(Guid id);
    }
}
