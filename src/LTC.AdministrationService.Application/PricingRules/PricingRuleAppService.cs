using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
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
        private readonly IRepository<Cinema, Guid> _cinemaRepository;
        private readonly IRepository<SeatType, Guid> _seatTypeRepository;

        public PricingRuleAppService(
            IRepository<PricingRule, Guid> repository,
            IRepository<Cinema, Guid> cinemaRepository,
            IRepository<SeatType, Guid> seatTypeRepository)
        {
            _repository = repository;
            _cinemaRepository = cinemaRepository;
            _seatTypeRepository = seatTypeRepository;
        }

        public async Task<PagedResultDto<PricingRuleOutputDto>> GetPricingRuleListAsync(Guid cinemaId, int skipCount, int maxResultCount)
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

        public async Task<PricingRuleOutputDto> CreatePricingRuleAsync(Guid cinemaId, CreatePricingRuleDto input)
        {
            ValidateInput(input);
            await ValidateOwnershipAsync(cinemaId, input.SeatTypeId);
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

            try
            {
                await _repository.InsertAsync(entity, true);
            }
            catch (DbUpdateException ex)
            {
                var reason = ex.GetBaseException().Message;
                throw new UserFriendlyException(
                    $"Unable to create pricing rule. Check DayOfWeek column migration and seat type/cinema mapping. Details: {reason}");
            }

            return ToOutputDto(entity);
        }

        public async Task<PricingRuleOutputDto> UpdatePricingRuleAsync(Guid id, CreatePricingRuleDto input)
        {
            ValidateInput(input);
            var entity = await _repository.GetAsync(id);
            await ValidateOwnershipAsync(entity.CinemaId, input.SeatTypeId);
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

            try
            {
                await _repository.UpdateAsync(entity, true);
            }
            catch (DbUpdateException ex)
            {
                var reason = ex.GetBaseException().Message;
                throw new UserFriendlyException(
                    $"Unable to update pricing rule. Check DayOfWeek column migration and seat type/cinema mapping. Details: {reason}");
            }

            return ToOutputDto(entity);
        }

        public async Task DeletePricingRuleAsync(Guid id)
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

        private async Task ValidateOwnershipAsync(Guid cinemaId, Guid? seatTypeId)
        {
            var cinema = await _cinemaRepository.FindAsync(cinemaId);
            if (cinema == null)
            {
                throw new UserFriendlyException("Cinema not found.");
            }

            if (CurrentTenant.Id.HasValue && cinema.TenantId != CurrentTenant.Id)
            {
                throw new UserFriendlyException("You can only manage pricing rules in your tenant.");
            }

            if (CurrentUser.Id.HasValue && cinema.ManagerUserId.HasValue && cinema.ManagerUserId != CurrentUser.Id)
            {
                throw new UserFriendlyException("You can only manage pricing rules in your assigned cinema.");
            }

            if (!seatTypeId.HasValue)
            {
                return;
            }

            var seatType = await _seatTypeRepository.FindAsync(seatTypeId.Value);
            if (seatType == null)
            {
                throw new UserFriendlyException("Seat type not found.");
            }

            if (CurrentTenant.Id.HasValue && seatType.TenantId != CurrentTenant.Id)
            {
                throw new UserFriendlyException("Selected seat type does not belong to your tenant.");
            }
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