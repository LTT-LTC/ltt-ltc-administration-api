using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class RevenueSnapshot : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid CinemaId { get; set; }
        public DateTime SnapshotDate { get; set; }
        public string? Granularity { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalTickets { get; set; }
        public decimal OccupancyRate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
