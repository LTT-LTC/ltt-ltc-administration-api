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
        public DateTime ShowDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? Duration { get; set; }
        public decimal BasePrice { get; set; }
        public string? Status { get; set; }
    }
}