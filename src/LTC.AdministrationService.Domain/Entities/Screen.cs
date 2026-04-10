using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class Screen : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid CinemaId { get; set; }
        public int ScreenNumber { get; set; }
        public string? ScreenType { get; set; }
        public string? SeatLayout { get; set; }
        public int SeatCount { get; set; }
        public string? Status { get; set;}
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
