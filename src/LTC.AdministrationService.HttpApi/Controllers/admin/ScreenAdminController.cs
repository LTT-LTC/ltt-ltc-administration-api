using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Screens;
using LTC.AdministrationService.Admin.Screens.Dtos.Input;
using LTC.AdministrationService.Admin.Screens.Dtos.Output;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin;

/// <summary>
/// Admin Screen operations: read-only (list and detail).
/// </summary>
[Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/screens")]
public class ScreenAdminController : AdminControllerBase
{
    private readonly IAdminScreenAppService _screenAppService;

    public ScreenAdminController(IAdminScreenAppService screenAppService)
    {
        _screenAppService = screenAppService;
    }

    [HttpGet("cinema/{cinemaId}")]
    [HttpGet("cinema/{cinemaId}/screen-all")]
    public virtual async Task<PagedResultDto<ScreenOutputDto>> GetScreenListAsync(Guid cinemaId, [FromQuery] GetScreenListInputDto input)
    {
        return await _screenAppService.GetScreenListAsync(cinemaId, input);
    }

    [HttpGet("{id}")]
    public virtual async Task<ScreenOutputDto> GetScreenAsync(Guid id)
    {
        return await _screenAppService.GetScreenAsync(id);
    }
}
