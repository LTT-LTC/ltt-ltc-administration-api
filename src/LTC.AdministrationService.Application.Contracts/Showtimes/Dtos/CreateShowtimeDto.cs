using System;

namespace LTC.AdministrationService.Showtimes.Dtos
{
    public class CreateShowtimeDto
    {
        public Guid MovieId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid DistributionId { get; set; }
        public string? MovieFormat { get; set; }
        public Guid ScreenId { get; set; }

        /// <summary>Optional JSON snapshot from manager FE (typically copied from the screen seat layout).</summary>
        public string? SeatLayout { get; set; }

        /// <summary>Legacy: cinema seat map template; layout JSON copied when FE does not send SeatLayout.</summary>
        public Guid? SeatMapId { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? Duration { get; set; }
        public decimal BasePrice { get; set; }
        public string? Status { get; set; }
    }
}