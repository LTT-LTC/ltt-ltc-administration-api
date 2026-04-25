using System;

namespace LTC.AdministrationService.PricingRules.Dtos
{
    public class CreatePricingRuleDto
    {
        public Guid? SeatTypeId { get; set; }
        public string RuleType { get; set; } 
        public decimal Multiplier { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string[]? DaysOfWeek { get; set; }
        public int Priority { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public bool IsActive { get; set; }
    }
}