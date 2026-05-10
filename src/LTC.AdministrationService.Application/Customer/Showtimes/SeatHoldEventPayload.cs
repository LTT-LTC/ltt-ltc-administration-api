using System;
using System.Collections.Generic;

namespace LTC.AdministrationService.Customer.Showtimes;

public class SeatHoldEventPayload
{
    public string EventType { get; set; } = string.Empty;

    public Guid ShowtimeId { get; set; }

    public string SessionKey { get; set; } = string.Empty;

    public List<string> SeatCodes { get; set; } = new();

    public DateTimeOffset? ExpiresAtUtc { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; }
}
