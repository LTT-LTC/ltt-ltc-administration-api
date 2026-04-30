using System;
using System.Threading.Tasks;
using LTC.AdministrationService.PricingRules;
using LTC.AdministrationService.PricingRules.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Customer;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/customer/pricing")]
public class PricingCustomerController : CustomerControllerBase
{
    private readonly IPricingRuleAppService _appService;

    public PricingCustomerController(IPricingRuleAppService appService)
    {
        _appService = appService;
    }

    [Route("cinema/{cinemaId}/rule")]
    [HttpGet]
    public virtual Task<PagedResultDto<PricingRuleOutputDto>> GetPricingRuleListAsync(Guid cinemaId, int skipCount = 0, int maxResultCount = 10)
        => _appService.GetPricingRuleListAsync(cinemaId, skipCount, maxResultCount);
}
