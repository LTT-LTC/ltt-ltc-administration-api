using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Customer.Showtimes.Dtos.Input;

public class HoldSeatsInputDto
{
    [Required]
    public Guid ShowtimeId { get; set; }

    [Required]
    [MinLength(1)]
    public List<string> SeatCodes { get; set; } = new();

    /// <summary>Browser booking session id (same as URL booking id).</summary>
    [Required]
    [MaxLength(128)]
    public string SessionKey { get; set; } = string.Empty;
}
