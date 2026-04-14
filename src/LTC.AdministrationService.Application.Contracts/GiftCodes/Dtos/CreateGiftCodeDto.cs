using System;

namespace LTC.AdministrationService.GiftCodes.Dtos
{
    public class CreateGiftCodeDto
    {
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int? PerUserLimit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}