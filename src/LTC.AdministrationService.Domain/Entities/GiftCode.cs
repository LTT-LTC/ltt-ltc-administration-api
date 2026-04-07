using System;
using Volo.Abp.Domain.Entities;

namespace LTC.AdministrationService.Entities
{
    public class GiftCode : Entity<Guid>
    {
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public int? PerUserLimit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}
