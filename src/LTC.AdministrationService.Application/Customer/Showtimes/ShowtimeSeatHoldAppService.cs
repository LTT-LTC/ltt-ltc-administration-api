using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Input;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Output;
using LTC.Shared.Hosting.Microservices.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;

namespace LTC.AdministrationService.Customer.Showtimes;

public class ShowtimeSeatHoldAppService : AdministrationServiceAppService, IShowtimeSeatHoldAppService
{
    private readonly ShowtimeSeatHoldStore _store;
    private readonly ISeatHoldRealtimeNotifier _realtimeNotifier;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IOptions<RabbitMqOptions> _rabbitMqOptions;
    private readonly IOptions<SeatHoldOptions> _holdOptions;
    private readonly ILogger<ShowtimeSeatHoldAppService> _logger;

    public ShowtimeSeatHoldAppService(
        ShowtimeSeatHoldStore store,
        ISeatHoldRealtimeNotifier realtimeNotifier,
        IOptions<SeatHoldOptions> holdOptions,
        ILogger<ShowtimeSeatHoldAppService> logger,
        IMessagePublisher messagePublisher,
        IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _store = store;
        _realtimeNotifier = realtimeNotifier;
        _holdOptions = holdOptions;
        _logger = logger;
        _messagePublisher = messagePublisher;
        _rabbitMqOptions = rabbitMqOptions;
    }

    public async Task<HoldSeatsOutputDto> HoldAsync(HoldSeatsInputDto input)
    {
        if (input.SeatCodes == null || input.SeatCodes.Count == 0)
        {
            throw new UserFriendlyException(L["NotEmpty", "SeatCodes"]);
        }

        var sessionKey = input.SessionKey?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(sessionKey))
        {
            throw new UserFriendlyException(L["NotEmpty", "SessionKey"]);
        }

        var minutes = Math.Clamp(_holdOptions.Value.HoldDurationMinutes, 5, 30);
        var ttl = TimeSpan.FromMinutes(minutes);

        var (ok, conflictSeat) = await _store.TryHoldAsync(input.ShowtimeId, input.SeatCodes, sessionKey, ttl);
        if (!ok)
        {
            throw new UserFriendlyException(L["SeatHoldConflict", conflictSeat ?? ""]);
        }

        var heldUntil = DateTime.UtcNow.Add(ttl);

        var seats = input.SeatCodes.Select(s => s.Trim().ToUpperInvariant()).Distinct().ToList();

        await _realtimeNotifier.NotifyAsync(new SeatHoldRealtimeNotification
        {
            ShowtimeId = input.ShowtimeId,
            Kind = "held",
            SeatCodes = seats,
            SessionKey = sessionKey,
            ExpiresAtUtc = new DateTimeOffset(heldUntil, TimeSpan.Zero),
        });

        await PublishSeatHoldEventAsync("SeatHoldGranted", input.ShowtimeId, sessionKey, seats, heldUntil);

        return new HoldSeatsOutputDto
        {
            HeldUntilUtc = heldUntil,
            HoldDurationMinutes = minutes,
        };
    }

    public async Task ReleaseAsync(Guid showtimeId, string sessionKey)
    {
        var key = sessionKey?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        var releasedSeats = await _store.ReleaseAsync(showtimeId, key);

        await _realtimeNotifier.NotifyAsync(new SeatHoldRealtimeNotification
        {
            ShowtimeId = showtimeId,
            Kind = "released",
            SeatCodes = releasedSeats,
            SessionKey = key,
            ExpiresAtUtc = null,
        });

        await PublishSeatHoldEventAsync("SeatHoldReleased", showtimeId, key, releasedSeats, null);
    }

    private async Task PublishSeatHoldEventAsync(
        string eventType,
        Guid showtimeId,
        string sessionKey,
        IReadOnlyList<string> seatCodes,
        DateTime? expiresUtc)
    {
        var routingKey = _rabbitMqOptions.Value.RoutingKeys.SeatHoldEvents;
        if (string.IsNullOrEmpty(routingKey))
        {
            return;
        }

        try
        {
            var payload = new SeatHoldEventPayload
            {
                EventType = eventType,
                ShowtimeId = showtimeId,
                SessionKey = sessionKey,
                SeatCodes = seatCodes.ToList(),
                ExpiresAtUtc = expiresUtc.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(expiresUtc.Value, DateTimeKind.Utc)) : null,
                OccurredAtUtc = DateTimeOffset.UtcNow,
            };

            await _messagePublisher.PublishAsync(routingKey, $"{showtimeId}:{sessionKey}", payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "RabbitMQ publish failed for seat hold event {EventType}", eventType);
        }
    }
}
