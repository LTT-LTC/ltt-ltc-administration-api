using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using LTC.AdministrationService.Admin.Screen;
using LTC.AdministrationService.Admin.Screen.Dtos.Input;
using LTC.AdministrationService.Admin.Screen.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;
using LTC.AdministrationService.Controllers;

namespace LTC.AdministrationService.Controllers
{
    [RemoteService]
    [Area("administration")]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/screens")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class ScreenController : AdministrationServiceController
    {
        private readonly IAdminScreenAppService _screenAppService;

        public ScreenController(IAdminScreenAppService screenAppService)
        {
            _screenAppService = screenAppService;
        }

        [HttpGet("cinema/{cinemaId}")]
        [HttpGet("cinema/{cinemaId}/screen-all")]
        public virtual async Task<PagedResultDto<ScreenOutputDto>> GetListAsync(Guid cinemaId, [FromQuery] GetScreenListInputDto input)
        {
            return await _screenAppService.GetListAsync(cinemaId, input);
        }

        [HttpGet("{id}")]
        public virtual async Task<ScreenOutputDto> GetAsync(Guid id)
        {
            return await _screenAppService.GetAsync(id);
        }

        [HttpPost("cinema/{cinemaId}")]
        public virtual async Task<ScreenOutputDto> CreateAsync(Guid cinemaId, [FromBody] CreateScreenInputDto input)
        {
            return await _screenAppService.CreateAsync(cinemaId, input);
        }

        [HttpPut("{id}")]
        public virtual async Task<ScreenOutputDto> UpdateAsync(Guid id, [FromBody] UpdateScreenInputDto input)
        {
            return await _screenAppService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _screenAppService.DeleteAsync(id);
        }
    }
}
