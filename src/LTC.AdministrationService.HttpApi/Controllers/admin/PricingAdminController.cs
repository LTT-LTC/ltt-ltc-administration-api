using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.PricingRules;
using LTC.AdministrationService.PricingRules.Dtos;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only PricingRule operations: all CRUD including delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/pricing")]
    public class PricingAdminController : AdminControllerBase
    {
        private readonly IPricingRuleAppService _appService;

        public PricingAdminController(IPricingRuleAppService appService) => _appService = appService;

        [Route("cinema/{cinemaId}/rule")]
        [HttpGet]
        public virtual Task<PagedResultDto<PricingRuleOutputDto>> GetPricingRuleListAsync(Guid cinemaId, int skipCount = 0, int maxResultCount = 10)
            => _appService.GetPricingRuleListAsync(cinemaId, skipCount, maxResultCount);

        [Route("cinema/{cinemaId}/rule")]
        [HttpPost]
        public virtual Task<PricingRuleOutputDto> CreatePricingRuleAsync(Guid cinemaId, CreatePricingRuleDto input)
            => _appService.CreatePricingRuleAsync(cinemaId, input);

        [Route("rule/{id}")]
        [HttpPut]
        public virtual Task<PricingRuleOutputDto> UpdatePricingRuleAsync(Guid id, CreatePricingRuleDto input)
            => _appService.UpdatePricingRuleAsync(id, input);

        [Route("rule/{id}")]
        [HttpDelete]
        public virtual Task DeletePricingRuleAsync(Guid id)
            => _appService.DeletePricingRuleAsync(id);
    }
}
