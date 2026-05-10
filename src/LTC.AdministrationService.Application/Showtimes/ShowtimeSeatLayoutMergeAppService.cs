using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Showtimes.Dtos;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Showtimes;

public class ShowtimeSeatLayoutMergeAppService : ApplicationService, IShowtimeSeatLayoutMergeAppService
{
    private readonly IRepository<Showtime, Guid> _repository;
    private readonly IDataFilter _dataFilter;
    private readonly ILogger<ShowtimeSeatLayoutMergeAppService> _logger;

    public ShowtimeSeatLayoutMergeAppService(
        IRepository<Showtime, Guid> repository,
        IDataFilter dataFilter,
        ILogger<ShowtimeSeatLayoutMergeAppService> logger)
    {
        _repository = repository;
        _dataFilter = dataFilter;
        _logger = logger;
    }

    public async Task MergePaidSeatsAsync(Guid showtimeId, MergePaidShowtimeSeatsInputDto input)
    {
        var codes = new HashSet<string>(
            (input.SeatCodes ?? new List<string>())
                .Select(NormalizeSeatCode)
                .Where(c => c.Length > 0),
            StringComparer.OrdinalIgnoreCase);

        if (codes.Count == 0)
        {
            _logger.LogWarning("Showtime seat merge skipped for {ShowtimeId}: empty seat code list.", showtimeId);
            return;
        }

        using (_dataFilter.Disable<IMultiTenant>())
        {
            var entity = await _repository.FindAsync(showtimeId);
            if (entity == null)
            {
                _logger.LogWarning("Showtime seat merge: showtime {ShowtimeId} not found.", showtimeId);
                return;
            }

            if (string.IsNullOrWhiteSpace(entity.SeatLayout))
            {
                _logger.LogWarning("Showtime seat merge: showtime {ShowtimeId} has no SeatLayout JSON.", showtimeId);
                return;
            }

            JsonNode? root;
            try
            {
                root = JsonNode.Parse(entity.SeatLayout);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Showtime seat merge: invalid JSON on showtime {ShowtimeId}.", showtimeId);
                return;
            }

            if (root is null)
            {
                return;
            }

            var rows = root["rows"]?.AsArray();
            if (rows == null || rows.Count == 0)
            {
                _logger.LogWarning("Showtime seat merge: showtime {ShowtimeId} layout has no rows.", showtimeId);
                return;
            }

            foreach (var row in rows)
            {
                var seats = row?["seats"]?.AsArray();
                if (seats == null)
                {
                    continue;
                }

                foreach (var seat in seats)
                {
                    var code = seat?["seatCode"]?.GetValue<string>();
                    if (string.IsNullOrWhiteSpace(code))
                    {
                        continue;
                    }

                    if (!codes.Contains(NormalizeSeatCode(code)))
                    {
                        continue;
                    }

                    seat["bookingStatus"] = "sold";
                }
            }

            entity.SeatLayout = root.ToJsonString(SerializerOptions);
            entity.UpdatedAt = Clock.Now;
            await _repository.UpdateAsync(entity, autoSave: true);
        }
    }

    private static string NormalizeSeatCode(string code) => code.Trim().ToUpperInvariant();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };
}
