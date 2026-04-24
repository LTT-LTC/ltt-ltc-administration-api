using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.PricingRules.Dtos;

namespace LTC.AdministrationService.PricingRules
{
    public interface IPricingRuleAppService : IApplicationService
    {
        Task<PagedResultDto<PricingRuleOutputDto>> GetListAsync(Guid cinemaId, int skipCount, int maxResultCount);
        Task<PricingRuleOutputDto> CreateAsync(Guid cinemaId, CreatePricingRuleDto input);
        Task<PricingRuleOutputDto> UpdateAsync(Guid id, CreatePricingRuleDto input);
        Task DeleteAsync(Guid id);
    }
}