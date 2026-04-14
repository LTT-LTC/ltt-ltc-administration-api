using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class PricingRule : Entity<Guid>, IMultiTenant
    {
        public PricingRule()
        {
        }

        public PricingRule(Guid id) : base(id)
        {
        }

        public Guid? TenantId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid? SeatTypeId { get; set; }
        public string? RuleType { get; set; }
        public decimal Multiplier { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? DayOfWeek { get; set; }
        public int Priority { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
