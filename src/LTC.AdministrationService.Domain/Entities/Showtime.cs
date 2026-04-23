using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class Showtime : Entity<Guid>, IMultiTenant
    {
        public Showtime() { }
        
        public Showtime(Guid id) : base(id) { }

        public Guid? TenantId { get; set; }
        public Guid MovieId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid DistributionId { get; set; }
        public string? MovieFormat { get; set; }
        public Guid ScreenId { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal BasePrice { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
