using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class SeatType : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int NumberOfSeat { get; set; }
        public string? DisplayDirection { get; set; }
        public decimal PriceMultiplier { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
