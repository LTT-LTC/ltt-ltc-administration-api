using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Redis;

namespace LTC.AdministrationService.Customer.Showtimes;

/// <summary>Redis-backed temporary seat locks per showtime (SET NX + TTL).</summary>
public class ShowtimeSeatHoldStore
{
    private readonly IConnectionMultiplexer _mux;
    private readonly string _keyPrefix;

    public ShowtimeSeatHoldStore(IConnectionMultiplexer mux, IWebHostEnvironment env)
    {
        _mux = mux;
        _keyPrefix = $"{env.ApplicationName}:seat-hold:";
    }

    private IDatabase Db => _mux.GetDatabase();

    private static string NormalizeSeat(string seatCode) => seatCode.Trim().ToUpperInvariant();

    private RedisKey SeatKey(Guid showtimeId, string seatCode) =>
        $"{_keyPrefix}st:{showtimeId:N}:seat:{NormalizeSeat(seatCode)}";

    private RedisKey ManifestKey(Guid showtimeId, string sessionKey) =>
        $"{_keyPrefix}manifest:{showtimeId:N}:{sessionKey}";

    /// <summary>
    /// Acquires holds for all seats or rolls back new keys if any seat is taken by another session.
    /// Same session refreshes TTL on seats it already holds.
    /// </summary>
    public async Task<(bool Success, string? ConflictSeat)> TryHoldAsync(
        Guid showtimeId,
        IReadOnlyList<string> seatCodes,
        string sessionKey,
        TimeSpan ttl)
    {
        var distinct = seatCodes.Select(NormalizeSeat).Distinct().ToList();
        var acquiredNew = new List<RedisKey>();

        foreach (var seat in distinct)
        {
            var key = SeatKey(showtimeId, seat);
            var existing = await Db.StringGetAsync(key);

            if (existing.HasValue && existing.ToString() == sessionKey)
            {
                await Db.KeyExpireAsync(key, ttl);
                continue;
            }

            if (existing.HasValue)
            {
                await RollbackAsync(acquiredNew);
                return (false, seat);
            }

            var ok = await Db.StringSetAsync(key, sessionKey, ttl, When.NotExists);
            if (ok)
            {
                acquiredNew.Add(key);
                continue;
            }

            // Lost race: another holder or our refresh path
            existing = await Db.StringGetAsync(key);
            if (existing.HasValue && existing.ToString() == sessionKey)
            {
                await Db.KeyExpireAsync(key, ttl);
                continue;
            }

            await RollbackAsync(acquiredNew);
            return (false, seat);
        }

        var manifest = JsonSerializer.Serialize(distinct);
        try
        {
            await Db.StringSetAsync(ManifestKey(showtimeId, sessionKey), manifest, ttl);
        }
        catch
        {
            await RollbackAsync(acquiredNew);
            throw;
        }

        return (true, null);
    }

    private async Task RollbackAsync(List<RedisKey> acquired)
    {
        foreach (var key in acquired)
        {
            await Db.KeyDeleteAsync(key);
        }
    }

    /// <summary>Deletes seat keys only when value matches sessionKey; removes manifest.</summary>
    public async Task<IReadOnlyList<string>> ReleaseAsync(Guid showtimeId, string sessionKey)
    {
        var manifestKey = ManifestKey(showtimeId, sessionKey);
        var manifestRaw = await Db.StringGetAsync(manifestKey);
        await Db.KeyDeleteAsync(manifestKey);

        if (manifestRaw.IsNullOrEmpty)
        {
            return Array.Empty<string>();
        }

        List<string>? seats;
        try
        {
            seats = JsonSerializer.Deserialize<List<string>>(manifestRaw.ToString());
        }
        catch
        {
            return Array.Empty<string>();
        }

        if (seats == null || seats.Count == 0)
        {
            return Array.Empty<string>();
        }

        var released = new List<string>();
        foreach (var seat in seats)
        {
            var key = SeatKey(showtimeId, seat);
            var val = await Db.StringGetAsync(key);
            if (val.HasValue && val.ToString() == sessionKey)
            {
                await Db.KeyDeleteAsync(key);
                released.Add(NormalizeSeat(seat));
            }
        }

        return released;
    }
}
