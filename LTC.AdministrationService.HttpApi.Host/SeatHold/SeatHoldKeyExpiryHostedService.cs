using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace LTC.AdministrationService.SeatHold;

/// <summary>
/// Forwards Redis key expirations for seat-hold keys to SignalR so other browsers refresh without polling.
/// Requires Redis <c>notify-keyspace-events</c> to include expiry (e.g. Ex).
/// </summary>
public sealed class SeatHoldKeyExpiryHostedService : BackgroundService
{
    private static readonly Regex SeatKeyRegex = new(
        @":seat-hold:st:([0-9a-fA-F\-]{36}):seat:(.+)$",
        RegexOptions.Compiled);

    private readonly IConnectionMultiplexer _mux;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SeatHoldKeyExpiryHostedService> _logger;

    public SeatHoldKeyExpiryHostedService(
        IConnectionMultiplexer mux,
        IServiceScopeFactory scopeFactory,
        ILogger<SeatHoldKeyExpiryHostedService> logger)
    {
        _mux = mux;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _mux.GetSubscriber();
        var channel = RedisChannel.Literal("__keyevent@0__:expired");

        void OnExpired(RedisChannel redisChannel, RedisValue value)
        {
            var key = value.ToString();
            _ = ProcessExpiredAsync(key, stoppingToken);
        }

        await subscriber.SubscribeAsync(channel, OnExpired, CommandFlags.None);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        finally
        {
            await subscriber.UnsubscribeAsync(channel, OnExpired, CommandFlags.None);
        }
    }

    private async Task ProcessExpiredAsync(string? key, CancellationToken stoppingToken)
    {
        try
        {
            if (string.IsNullOrEmpty(key) || !key.Contains(":seat-hold:st:", StringComparison.Ordinal))
            {
                return;
            }

            var m = SeatKeyRegex.Match(key);
            if (!m.Success || !Guid.TryParse(m.Groups[1].Value, out var showtimeId))
            {
                return;
            }

            var seatCode = m.Groups[2].Value.Trim();
            if (string.IsNullOrEmpty(seatCode))
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var notifier = scope.ServiceProvider.GetRequiredService<ISeatHoldRealtimeNotifier>();
            await notifier.NotifyAsync(new SeatHoldRealtimeNotification
            {
                ShowtimeId = showtimeId,
                Kind = "expired",
                SeatCodes = new[] { seatCode },
                SessionKey = null,
                ExpiresAtUtc = null,
            }, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Seat hold expiry forward skipped or failed for key {Key}", key);
        }
    }
}
