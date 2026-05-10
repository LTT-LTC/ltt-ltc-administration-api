using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LTC.AdministrationService.Dashboard.Dtos;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Movies;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace LTC.AdministrationService.Dashboard;

public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private readonly IRepository<Showtime, Guid> _showtimeRepository;
    private readonly IRepository<Screen, Guid> _screenRepository;
    private readonly IRepository<GiftCode, Guid> _giftCodeRepository;
    private readonly IMovieLookupClient _movieLookupClient;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public DashboardAppService(
        IRepository<Showtime, Guid> showtimeRepository,
        IRepository<Screen, Guid> screenRepository,
        IRepository<GiftCode, Guid> giftCodeRepository,
        IMovieLookupClient movieLookupClient)
    {
        _showtimeRepository = showtimeRepository;
        _screenRepository = screenRepository;
        _giftCodeRepository = giftCodeRepository;
        _movieLookupClient = movieLookupClient;
    }

    public async Task<HallOccupancyOutputDto> GetHallOccupancyAsync(DashboardFilterInputDto input)
    {
        var showtimeQuery = await _showtimeRepository.GetQueryableAsync();

        var fromDate = input.FromDate.Date;
        var toDate = input.ToDate.Date;
        showtimeQuery = showtimeQuery.Where(s => s.ShowDate >= fromDate && s.ShowDate <= toDate);

        if (input.CinemaId.HasValue)
            showtimeQuery = showtimeQuery.Where(s => s.CinemaId == input.CinemaId.Value);

        showtimeQuery = showtimeQuery.Where(s => s.Status != "Cancelled");

        var showtimes = await showtimeQuery.OrderBy(s => s.ShowDate).ThenBy(s => s.StartTime).ToListAsync();

        var screenIds = showtimes.Select(s => s.ScreenId).Distinct().ToList();
        var screenQuery = await _screenRepository.GetQueryableAsync();
        var screens = await screenQuery.Where(s => screenIds.Contains(s.Id)).ToListAsync();
        var screenDict = screens.ToDictionary(s => s.Id);

        var movieIds = showtimes.Select(s => s.MovieId).Distinct().ToList();
        var movieMap = await _movieLookupClient.GetByIdsAsync(movieIds, CurrentTenant.Id);

        var items = new List<HallOccupancyItemDto>();
        int totalSeatsAll = 0;
        int soldSeatsAll = 0;

        foreach (var st in showtimes)
        {
            var totalSeats = screenDict.TryGetValue(st.ScreenId, out var screen) ? screen.SeatCount : 0;
            var soldSeats = CountSoldSeats(st.SeatLayout, totalSeats);
            var movieTitle = movieMap.TryGetValue(st.MovieId, out var movie) ? movie.Title : null;
            var screenName = screen != null ? $"Hall {screen.ScreenNumber}" : "Unknown";

            totalSeatsAll += totalSeats;
            soldSeatsAll += soldSeats;

            items.Add(new HallOccupancyItemDto
            {
                ScreenId = st.ScreenId,
                ScreenName = screenName,
                MovieTitle = movieTitle ?? "Unknown",
                TotalSeats = totalSeats,
                SoldSeats = soldSeats,
                OccupancyPercent = totalSeats > 0 ? Math.Round((decimal)soldSeats / totalSeats * 100, 1) : 0,
                ShowDate = st.ShowDate,
                StartTime = st.StartTime.ToString(@"hh\:mm"),
            });
        }

        return new HallOccupancyOutputDto
        {
            Halls = items,
            AverageOccupancyRate = totalSeatsAll > 0 ? Math.Round((decimal)soldSeatsAll / totalSeatsAll * 100, 1) : 0,
        };
    }

    public async Task<PromotionSummaryOutputDto> GetPromotionSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        var giftCodeQuery = await _giftCodeRepository.GetQueryableAsync();
        var activeGiftCodes = await giftCodeQuery
            .Where(g => g.Status == "Active" || g.Status == "active")
            .ToListAsync();

        var activePromotions = activeGiftCodes
            .Where(g => g.EndDate == null || g.EndDate > now)
            .Select(g => new ActivePromotionDto
            {
                Name = g.Description ?? g.Code,
                Status = g.Status ?? "Active",
                ExpiresAt = g.EndDate,
                Badge = g.EndDate.HasValue && (g.EndDate.Value - now).TotalDays <= 3
                    ? $"Expires in {Math.Ceiling((g.EndDate.Value - now).TotalDays)} days"
                    : $"{g.UsageCount} used",
            })
            .Take(5)
            .ToList();

        var totalValue = activeGiftCodes.Sum(g => g.DiscountValue * Math.Max(0, (g.UsageLimit ?? 0) - g.UsageCount));
        var usedToday = activeGiftCodes.Count(g => g.UsageCount > 0);

        return new PromotionSummaryOutputDto
        {
            ActivePromotions = activePromotions,
            GiftCardSummary = new GiftCardSummaryDto
            {
                OutstandingBalance = totalValue,
                RedeemedToday = 0,
                ActiveGiftCards = activeGiftCodes.Count,
            },
        };
    }

    private static int CountSoldSeats(string? seatLayoutJson, int totalSeats)
    {
        if (string.IsNullOrEmpty(seatLayoutJson)) return 0;

        try
        {
            using var doc = JsonDocument.Parse(seatLayoutJson);
            int soldCount = 0;

            if (doc.RootElement.TryGetProperty("seats", out var seatsArray) && seatsArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var seat in seatsArray.EnumerateArray())
                {
                    if (seat.TryGetProperty("seatBookingStatus", out var status))
                    {
                        var statusStr = status.GetString();
                        if (statusStr == "Sold" || statusStr == "sold" || statusStr == "SOLD")
                            soldCount++;
                    }
                }
            }

            return soldCount;
        }
        catch
        {
            return 0;
        }
    }
}
