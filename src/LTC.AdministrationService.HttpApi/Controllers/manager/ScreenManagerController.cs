using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Screens;
using LTC.AdministrationService.Admin.Screens.Dtos.Input;
using LTC.AdministrationService.Admin.Screens.Dtos.Output;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager;

/// <summary>
/// Manager Screen operations: full CRUD plus list/detail.
/// </summary>
[Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/screens")]
public class ScreenManagerController : ManagerControllerBase
{
    private readonly IAdminScreenAppService _screenAppService;

    public ScreenManagerController(IAdminScreenAppService screenAppService)
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
