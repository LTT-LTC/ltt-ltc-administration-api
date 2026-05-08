using System;

namespace LTC.AdministrationService.Customer.Showtimes.Dtos.Output
{
    public class ShowtimeCustomerOutputDto
    {
        public Guid Id { get; set; }
        public Guid CinemaId { get; set; }
        public Guid MovieId { get; set; }
        public Guid ScreenId { get; set; }
        // ISO 8601 datetime composed from ShowDate + StartTime, kept as string for JSON compatibility.
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        // Format identifier; admin's Showtime entity stores a JSON descriptor in MovieFormat instead.
        public Guid FormatId { get; set; }
        public string? Status { get; set; }
        public string? MovieFormat { get; set; }
        public string? ScreenName { get; set; }
        // Movie metadata fields. Currently null because admin service does not call movie-service;
        // FE is expected to enrich client-side via customerMovieService when needed.
        public string? MovieTitle { get; set; }
        public string? OriginalTitle { get; set; }
        public string? PosterUrl { get; set; }
        public int? DurationMins { get; set; }
        public string? RatingCode { get; set; }
    }
}
