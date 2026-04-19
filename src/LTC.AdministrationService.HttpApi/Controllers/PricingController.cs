using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.PricingRules;
using LTC.AdministrationService.PricingRules.Dtos;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers
{
    [ApiController]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/pricing")]
    [Authorize(Roles = "Manager,Admin")]
    public class PricingController : AdministrationServiceController
    {
        private readonly IPricingRuleAppService _appService;
        public PricingController(IPricingRuleAppService appService) => _appService = appService;

        [Route("cinema/{cinemaId}/rule")]
        [HttpGet] public virtual Task<PagedResultDto<PricingRuleOutputDto>> GetListAsync(Guid cinemaId, int skipCount = 0, int maxResultCount = 10) 
            => _appService.GetListAsync(cinemaId, skipCount, maxResultCount);

        [Route("cinema/{cinemaId}/rule")]
        [HttpPost] public virtual Task<PricingRuleOutputDto> CreateAsync(Guid cinemaId, CreatePricingRuleDto input) 
            => _appService.CreateAsync(cinemaId, input);

        [Route("rule/{id}")]
        [HttpDelete] public virtual Task DeleteAsync(Guid id) 
            => _appService.DeleteAsync(id);
    }
}
