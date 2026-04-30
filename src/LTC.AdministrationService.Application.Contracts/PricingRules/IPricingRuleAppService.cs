using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.PricingRules.Dtos;

namespace LTC.AdministrationService.PricingRules
{
    public interface IPricingRuleAppService : IApplicationService
    {
        Task<PagedResultDto<PricingRuleOutputDto>> GetPricingRuleListAsync(Guid cinemaId, int skipCount, int maxResultCount);
        Task<PricingRuleOutputDto> CreatePricingRuleAsync(Guid cinemaId, CreatePricingRuleDto input);
        Task<PricingRuleOutputDto> UpdatePricingRuleAsync(Guid id, CreatePricingRuleDto input);
        Task DeletePricingRuleAsync(Guid id);
    }
}