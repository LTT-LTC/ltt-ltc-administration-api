using System;
using System.Collections.Generic;

namespace LTC.AdministrationService.Showtimes.Dtos;

/// <summary>
/// Input DTO for persisting seat holds in the showtime database when user confirms selection.
/// </summary>
public class HoldShowtimeSeatsInputDto
{
    public List<string> SeatCodes { get; set; } = new();

    /// <summary>
    /// Optional: When the hold should expire if payment is not completed.
    /// Used for potential cleanup processes.
    /// </summary>
    public DateTime? HoldExpiresAt { get; set; }
}
