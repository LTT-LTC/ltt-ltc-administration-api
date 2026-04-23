using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.SeatTypes;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Input;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Output;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager SeatType operations: read-only access to seat type list and details.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/seat-type")]
    public class SeatTypeManagerController : ManagerControllerBase
    {
        private readonly IAdminSeatTypeAppService _appService;

        public SeatTypeManagerController(IAdminSeatTypeAppService appService)
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
    }
}
