using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.SeatTypes;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Input;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Output;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only SeatType operations: create, update, delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/seat-type")]
    public class SeatTypeAdminController : AdminControllerBase
    {
        private readonly IAdminSeatTypeAppService _appService;

        public SeatTypeAdminController(IAdminSeatTypeAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public virtual async Task<PagedResultDto<SeatTypeOutputDto>> GetSeatTypeListAsync([FromQuery] GetSeatTypeListInputDto input)
        {
            return await _appService.GetSeatTypeListAsync(input);
        }

        [HttpGet("{id}")]
        public virtual async Task<SeatTypeOutputDto> GetSeatTypeAsync(Guid id)
        {
            return await _appService.GetSeatTypeAsync(id);
        }

        [HttpPost]
        public virtual async Task<SeatTypeOutputDto> CreateSeatTypeAsync([FromBody] CreateSeatTypeInputDto input)
        {
            return await _appService.CreateSeatTypeAsync(input);
        }

        [HttpPut("{id}")]
        public virtual async Task<SeatTypeOutputDto> UpdateSeatTypeAsync(Guid id, [FromBody] UpdateSeatTypeInputDto input)
        {
            return await _appService.UpdateSeatTypeAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteSeatTypeAsync(Guid id)
        {
            await _appService.DeleteSeatTypeAsync(id);
        }
    }
}
