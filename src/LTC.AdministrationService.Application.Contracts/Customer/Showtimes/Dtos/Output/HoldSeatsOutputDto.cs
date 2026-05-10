using System;

namespace LTC.AdministrationService.Customer.Showtimes.Dtos.Output;

public class HoldSeatsOutputDto
{
    public DateTime HeldUntilUtc { get; set; }

    public int HoldDurationMinutes { get; set; }
}
