using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Admin.SeatMaps;
using LTC.AdministrationService.Admin.SeatMaps.Dtos.Input;
using LTC.AdministrationService.Admin.SeatMaps.Dtos.Output;
using LTC.AdministrationService.Controllers.Manager;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Manager
{
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/seatmaps")]
    public class SeatMapManagerController : ManagerControllerBase
    {
        private readonly IAdminSeatMapAppService _seatMapAppService;

        public SeatMapManagerController(IAdminSeatMapAppService seatMapAppService)
        {
            _seatMapAppService = seatMapAppService;
        }

        [HttpGet("cinema/{cinemaId}")]
        public virtual async Task<PagedResultDto<SeatMapOutputDto>> GetSeatMapListAsync(Guid cinemaId, [FromQuery] GetSeatMapListInputDto input)
        {
            return await _seatMapAppService.GetSeatMapListAsync(cinemaId, input);
        }

        [HttpGet("{id}")]
        public virtual async Task<SeatMapOutputDto> GetSeatMapAsync(Guid id)
        {
            return await _seatMapAppService.GetSeatMapAsync(id);
        }

        [HttpPost("cinema/{cinemaId}")]
        public virtual async Task<SeatMapOutputDto> CreateSeatMapAsync(Guid cinemaId, [FromBody] CreateSeatMapInputDto input)
        {
            return await _seatMapAppService.CreateSeatMapAsync(cinemaId, input);
        }

        [HttpPut("{id}")]
        public virtual async Task<SeatMapOutputDto> UpdateSeatMapAsync(Guid id, [FromBody] UpdateSeatMapInputDto input)
        {
            return await _seatMapAppService.UpdateSeatMapAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteSeatMapAsync(Guid id)
        {
            await _seatMapAppService.DeleteSeatMapAsync(id);
        }
    }
}
