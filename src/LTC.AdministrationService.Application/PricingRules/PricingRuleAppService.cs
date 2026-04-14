using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.PricingRules.Dtos;

namespace LTC.AdministrationService.PricingRules
{
    public class PricingRuleAppService : ApplicationService, IPricingRuleAppService
    {
        private readonly IRepository<PricingRule, Guid> _repository;

        public PricingRuleAppService(IRepository<PricingRule, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<PricingRuleOutputDto>> GetListAsync(Guid cinemaId, int skipCount, int maxResultCount)
        {
            var query = await _repository.GetQueryableAsync();
            query = query.Where(e => e.CinemaId == cinemaId);

            var total = await query.CountAsync();
            var items = await query.Skip(skipCount).Take(maxResultCount).ToListAsync();

            return new PagedResultDto<PricingRuleOutputDto>(
                total,
                items.Select(x => ObjectMapper.Map<PricingRule, PricingRuleOutputDto>(x)).ToList()
            );
        }

        public async Task<PricingRuleOutputDto> CreateAsync(Guid cinemaId, CreatePricingRuleDto input)
        {
            var entity = new PricingRule(GuidGenerator.Create())
            {
                CinemaId = cinemaId,
                SeatTypeId = input.SeatTypeId,
                RuleType = input.RuleType,
                Multiplier = input.Multiplier,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                DayOfWeek = input.DayOfWeek,
                Priority = input.Priority
            };
            await _repository.InsertAsync(entity, true);
            return ObjectMapper.Map<PricingRule, PricingRuleOutputDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}