using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Dashboard;
using LTC.AdministrationService.Dashboard.Dtos;

namespace LTC.AdministrationService.Controllers.Manager;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/dashboard")]
public class DashboardManagerController : ManagerControllerBase
{
    private readonly IDashboardAppService _dashboardAppService;

    public DashboardManagerController(IDashboardAppService dashboardAppService)
    {
        _dashboardAppService = dashboardAppService;
    }

    [HttpGet("hall-occupancy")]
    public async Task<HallOccupancyOutputDto> GetHallOccupancyAsync([FromQuery] DashboardFilterInputDto input)
    {
        return await _dashboardAppService.GetHallOccupancyAsync(input);
    }

    [HttpGet("promotion-summary")]
    public async Task<PromotionSummaryOutputDto> GetPromotionSummaryAsync()
    {
        return await _dashboardAppService.GetPromotionSummaryAsync();
    }
}
