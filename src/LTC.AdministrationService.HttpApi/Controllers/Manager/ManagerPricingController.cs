using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.PricingRules;
using LTC.AdministrationService.PricingRules.Dtos;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.HttpApi.Controllers.Manager
{
    [ApiController]
    public class ManagerPricingController : AbpControllerBase
    {
        private readonly IPricingRuleAppService _appService;
        public ManagerPricingController(IPricingRuleAppService appService) => _appService = appService;

        [Route("ltc/administration-service/api/administration/manager/cinema/{cinemaId}/pricing-rule-all")]
        [HttpGet] public virtual Task<PagedResultDto<PricingRuleOutputDto>> GetListAsync(Guid cinemaId, int skipCount = 0, int maxResultCount = 10) 
            => _appService.GetListAsync(cinemaId, skipCount, maxResultCount);

        [Route("ltc/administration-service/api/administration/manager/cinema/{cinemaId}/pricing-rule")]
        [HttpPost] public virtual Task<PricingRuleOutputDto> CreateAsync(Guid cinemaId, CreatePricingRuleDto input) 
            => _appService.CreateAsync(cinemaId, input);

        [Route("ltc/administration-service/api/administration/manager/cinema/{cinemaId}/pricing-rule/{id}")]
        [HttpDelete] public virtual Task DeleteAsync(Guid id) 
            => _appService.DeleteAsync(id);
    }
}
