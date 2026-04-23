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
        public virtual async Task<PagedResultDto<SeatTypeOutputDto>> GetListAsync([FromQuery] GetSeatTypeListInputDto input)
        {
            return await _appService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public virtual async Task<SeatTypeOutputDto> GetAsync(Guid id)
        {
            return await _appService.GetAsync(id);
        }

        [HttpPost]
        public virtual async Task<SeatTypeOutputDto> CreateAsync([FromBody] CreateSeatTypeInputDto input)
        {
            return await _appService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public virtual async Task<SeatTypeOutputDto> UpdateAsync(Guid id, [FromBody] UpdateSeatTypeInputDto input)
        {
            return await _appService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _appService.DeleteAsync(id);
        }
    }
}
