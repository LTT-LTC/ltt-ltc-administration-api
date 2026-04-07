using System;
using Volo.Abp.Domain.Entities;

namespace LTC.AdministrationService.Entities
{
    public class SeatType : Entity<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PriceMultiplier { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
