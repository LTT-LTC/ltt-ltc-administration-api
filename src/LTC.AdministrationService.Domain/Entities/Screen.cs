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
        public int SeatCount { get; set; }
        /// <summary>
        /// JSON snapshot of the seat layout used by the booking process to determine
        /// seat type, selection state, exits, and entrances. Sourced from a chosen
        /// <see cref="SeatMap"/> template at create/update time.
        /// </summary>
        public string? SeatLayout { get; set; }
        public string? Status { get; set;}
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
