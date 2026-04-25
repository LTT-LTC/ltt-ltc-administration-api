using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
                items.Select(ToOutputDto).ToList()
            );
        }

        public async Task<PricingRuleOutputDto> CreateAsync(Guid cinemaId, CreatePricingRuleDto input)
        {
            ValidateInput(input);
            var entity = new PricingRule(GuidGenerator.Create())
            {
                CinemaId = cinemaId,
                SeatTypeId = input.SeatTypeId,
                RuleType = input.RuleType,
                Multiplier = input.Multiplier,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                DayOfWeek = SerializeDaysOfWeek(input.DaysOfWeek),
                Priority = input.Priority,
                ValidFrom = input.ValidFrom,
                ValidUntil = input.ValidUntil,
                IsActive = input.IsActive,
                CreatedAt = Clock.Now,
                UpdatedAt = Clock.Now
            };
            await _repository.InsertAsync(entity, true);
            return ToOutputDto(entity);
        }

        public async Task<PricingRuleOutputDto> UpdateAsync(Guid id, CreatePricingRuleDto input)
        {
            ValidateInput(input);
            var entity = await _repository.GetAsync(id);
            entity.SeatTypeId = input.SeatTypeId;
            entity.RuleType = input.RuleType;
            entity.Multiplier = input.Multiplier;
            entity.StartTime = input.StartTime;
            entity.EndTime = input.EndTime;
            entity.DayOfWeek = SerializeDaysOfWeek(input.DaysOfWeek);
            entity.Priority = input.Priority;
            entity.ValidFrom = input.ValidFrom;
            entity.ValidUntil = input.ValidUntil;
            entity.IsActive = input.IsActive;
            entity.UpdatedAt = Clock.Now;

            await _repository.UpdateAsync(entity, true);
            return ToOutputDto(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        private static void ValidateInput(CreatePricingRuleDto input)
        {
            if (string.IsNullOrWhiteSpace(input.RuleType))
            {
                throw new Volo.Abp.UserFriendlyException("Rule type is required.");
            }

            if (input.Multiplier <= 0)
            {
                throw new Volo.Abp.UserFriendlyException("Multiplier must be a positive number.");
            }

            if (input.ValidFrom.HasValue && input.ValidUntil.HasValue && input.ValidFrom.Value.Date > input.ValidUntil.Value.Date)
            {
                throw new Volo.Abp.UserFriendlyException("Valid from date cannot be later than valid until date.");
            }

            if (input.DaysOfWeek == null || input.DaysOfWeek.Length == 0)
            {
                throw new Volo.Abp.UserFriendlyException("At least one day token is required.");
            }

            var normalized = input.DaysOfWeek
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            if (normalized.Count == 0)
            {
                throw new Volo.Abp.UserFriendlyException("At least one valid day token is required.");
            }

            var allowed = new HashSet<string> { "ALL", "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
            if (normalized.Any(x => !allowed.Contains(x)))
            {
                throw new Volo.Abp.UserFriendlyException("Day-of-week token is invalid.");
            }
        }

        private static string SerializeDaysOfWeek(string[]? daysOfWeek)
        {
            var normalized = (daysOfWeek ?? Array.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            if (normalized.Contains("ALL"))
            {
                return JsonSerializer.Serialize(new[] { "ALL" });
            }

            return JsonSerializer.Serialize(normalized);
        }

        private static string[] ParseDaysOfWeek(string? serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                return Array.Empty<string>();
            }

            var raw = serialized.Trim();
            if (raw.StartsWith("["))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<string[]>(raw);
                    return parsed?.Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim().ToUpperInvariant())
                        .Distinct()
                        .ToArray() ?? Array.Empty<string>();
                }
                catch
                {
                    return Array.Empty<string>();
                }
            }

            if (int.TryParse(raw, out var legacyDay))
            {
                return legacyDay switch
                {
                    1 => new[] { "MON" },
                    2 => new[] { "TUE" },
                    3 => new[] { "WED" },
                    4 => new[] { "THU" },
                    5 => new[] { "FRI" },
                    6 => new[] { "SAT" },
                    0 => new[] { "SUN" },
                    _ => Array.Empty<string>()
                };
            }

            return new[] { raw.ToUpperInvariant() };
        }

        private static PricingRuleOutputDto ToOutputDto(PricingRule entity)
        {
            return new PricingRuleOutputDto
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                CinemaId = entity.CinemaId,
                SeatTypeId = entity.SeatTypeId,
                RuleType = entity.RuleType ?? string.Empty,
                Multiplier = entity.Multiplier,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DaysOfWeek = ParseDaysOfWeek(entity.DayOfWeek),
                Priority = entity.Priority,
                ValidFrom = entity.ValidFrom,
                ValidUntil = entity.ValidUntil,
                IsActive = entity.IsActive
            };
        }
    }
}