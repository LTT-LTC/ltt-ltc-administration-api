using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Admin.Screens;
using LTC.AdministrationService.Admin.Screens.Dtos.Input;
using LTC.AdministrationService.Admin.Screens.Dtos.Output;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Staff;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/staff/screens")]
public class ScreenStaffController : StaffControllerBase
{
    private readonly IAdminScreenAppService _screenAppService;

    public ScreenStaffController(IAdminScreenAppService screenAppService)
    {
        _screenAppService = screenAppService;
    }

    [HttpGet("cinema/{cinemaId}")]
    [HttpGet("cinema/{cinemaId}/screen-all")]
    public virtual Task<PagedResultDto<ScreenOutputDto>> GetScreenListAsync(Guid cinemaId, [FromQuery] GetScreenListInputDto input)
        => _screenAppService.GetScreenListAsync(cinemaId, input);

    [HttpGet("{id}")]
    public virtual Task<ScreenOutputDto> GetScreenAsync(Guid id)
        => _screenAppService.GetScreenAsync(id);
}
