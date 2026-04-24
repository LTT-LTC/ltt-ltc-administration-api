using System;
using System.Threading.Tasks;
using LTC.AdministrationService.PricingRules;
using LTC.AdministrationService.PricingRules.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Staff;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/staff/pricing")]
public class PricingStaffController : StaffControllerBase
{
    private readonly IPricingRuleAppService _appService;

    public PricingStaffController(IPricingRuleAppService appService)
    {
        _appService = appService;
    }

    [Route("cinema/{cinemaId}/rule")]
    [HttpGet]
    public virtual Task<PagedResultDto<PricingRuleOutputDto>> GetListAsync(Guid cinemaId, int skipCount = 0, int maxResultCount = 10)
        => _appService.GetListAsync(cinemaId, skipCount, maxResultCount);
}
