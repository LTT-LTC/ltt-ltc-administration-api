using System;
using System.Collections.Generic;

namespace LTC.AdministrationService.Customer.Showtimes;

public class SeatHoldRealtimeNotification
{
    public Guid ShowtimeId { get; set; }

    /// <summary>held | released | expired</summary>
    public string Kind { get; set; } = string.Empty;

    public IReadOnlyList<string> SeatCodes { get; set; } = Array.Empty<string>();

    public string? SessionKey { get; set; }

    public DateTimeOffset? ExpiresAtUtc { get; set; }
}
