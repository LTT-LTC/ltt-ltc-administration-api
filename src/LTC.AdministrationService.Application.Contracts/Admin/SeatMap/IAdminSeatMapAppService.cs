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
        Task<PagedResultDto<SeatMapOutputDto>> GetSeatMapListAsync(Guid cinemaId, GetSeatMapListInputDto input);
        Task<SeatMapOutputDto> GetSeatMapAsync(Guid id);
        Task<SeatMapOutputDto> CreateSeatMapAsync(Guid cinemaId, CreateSeatMapInputDto input);
        Task<SeatMapOutputDto> UpdateSeatMapAsync(Guid id, UpdateSeatMapInputDto input);
        Task DeleteSeatMapAsync(Guid id);
    }
}
