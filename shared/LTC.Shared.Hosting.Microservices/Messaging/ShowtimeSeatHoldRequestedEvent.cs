using System;
using System.Collections.Generic;

namespace LTC.Shared.Hosting.Microservices.Messaging;

/// <summary>
/// Event published when a user confirms seat selection (before payment).
/// This persists the seat hold in the showtime database as "held" status.
/// </summary>
public sealed class ShowtimeSeatHoldRequestedEvent
{
    public Guid BookingId { get; set; }

    public Guid? TenantId { get; set; }

    public Guid ShowtimeId { get; set; }

    public List<string> SeatCodes { get; set; } = [];

    /// <summary>Session key used for Redis seat holds (typically the booking ID).</summary>
    public string? SessionKey { get; set; }

    /// <summary>Duration after which the hold should auto-expire if payment not completed.</summary>
    public DateTime? HoldExpiresAt { get; set; }
}
