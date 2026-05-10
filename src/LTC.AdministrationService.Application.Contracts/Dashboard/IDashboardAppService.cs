using System.Threading.Tasks;
using LTC.AdministrationService.Dashboard.Dtos;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Dashboard;

public interface IDashboardAppService : IApplicationService
{
    Task<HallOccupancyOutputDto> GetHallOccupancyAsync(DashboardFilterInputDto input);
    Task<PromotionSummaryOutputDto> GetPromotionSummaryAsync();
}
