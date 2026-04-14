using System;

namespace LTC.AdministrationService.PricingRules.Dtos
{
    public class PricingRuleOutputDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid? SeatTypeId { get; set; }
        public string RuleType { get; set; } // SEAT_TYPE, SHOWTIME, FORMAT, DAYS_OF_WEEK, AGES
        public decimal Multiplier { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? DayOfWeek { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }
}