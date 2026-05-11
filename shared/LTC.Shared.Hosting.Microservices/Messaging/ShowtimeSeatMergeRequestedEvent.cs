using System;
using System.Collections.Generic;

namespace LTC.Shared.Hosting.Microservices.Messaging;

public sealed class ShowtimeSeatMergeRequestedEvent
{
    public Guid BookingId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid ShowtimeId { get; set; }

    public List<string> SeatCodes { get; set; } = [];

    /// <summary>Session key used for Redis seat holds (typically the booking ID). Allows explicit release after payment.</summary>
    public string? SessionKey { get; set; }
}
