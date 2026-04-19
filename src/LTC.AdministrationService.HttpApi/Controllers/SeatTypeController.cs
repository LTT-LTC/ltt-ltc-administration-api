using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.SeatTypes;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Input;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Controllers
{
    [ApiController]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/seat-type")]
    [Authorize(Roles = "Admin,Manager")]
    public class SeatTypeController : AbpControllerBase
    {
        private readonly IAdminSeatTypeAppService _appService;

        public SeatTypeController(IAdminSeatTypeAppService appService)
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