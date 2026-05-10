using System;
using System.Collections.Generic;
using LTC.AdministrationService.Showtimes.Dtos;

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
        // Optional duration sourced from the showtime entity itself (the embedded
        // Movie virtual object also exposes DurationMins).
        public int? DurationMins { get; set; }

        // Virtual object enriched at read time from the movie microservice. May be
        // null if the upstream lookup failed; callers should degrade gracefully.
        public MovieLookupDto? Movie { get; set; }

        /// <summary>Snapshot JSON persisted on the showtime (same shape as screen seat layout).</summary>
        public string? SeatLayout { get; set; }

        /// <summary>
        /// Seat codes marked sold in the persisted layout JSON (<c>bookingStatus: sold</c>), uppercase for UI blocking.
        /// </summary>
        public List<string> SoldSeatCodes { get; set; } = new();

        /// <summary>Seat codes currently held in Redis (temporary locks), normalized uppercase.</summary>
        public List<string> HeldSeatCodes { get; set; } = new();
    }
}
