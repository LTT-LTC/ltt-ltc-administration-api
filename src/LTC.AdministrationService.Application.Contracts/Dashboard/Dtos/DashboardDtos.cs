using System;
using System.Collections.Generic;

namespace LTC.AdministrationService.Dashboard.Dtos;

public class DashboardFilterInputDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? CinemaId { get; set; }
}

public class HallOccupancyOutputDto
{
    public List<HallOccupancyItemDto> Halls { get; set; } = new();
    public decimal AverageOccupancyRate { get; set; }
}

public class HallOccupancyItemDto
{
    public Guid ScreenId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public string? MovieTitle { get; set; }
    public int TotalSeats { get; set; }
    public int SoldSeats { get; set; }
    public decimal OccupancyPercent { get; set; }
    public DateTime ShowDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
}

public class PromotionSummaryOutputDto
{
    public List<ActivePromotionDto> ActivePromotions { get; set; } = new();
    public GiftCardSummaryDto GiftCardSummary { get; set; } = new();
}

public class ActivePromotionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Badge { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class GiftCardSummaryDto
{
    public decimal OutstandingBalance { get; set; }
    public decimal RedeemedToday { get; set; }
    public int ActiveGiftCards { get; set; }
}
