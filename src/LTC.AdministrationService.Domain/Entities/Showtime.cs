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

        /// <summary>Seat map template used when this showtime was scheduled (audit).</summary>
        public Guid? SeatMapId { get; set; }

        /// <summary>
        /// JSON snapshot of seat geometry + statuses (<c>seatBookingStatus</c>) at scheduling time;
        /// updated after successful payments with sold seats.
        /// </summary>
        public string? SeatLayout { get; set; }

        public DateTime ShowDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Duration { get; set; }
        public decimal BasePrice { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
