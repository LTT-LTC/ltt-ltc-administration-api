namespace LTC.AdministrationService.Customer.Showtimes;

public class SeatHoldOptions
{
    public const string SectionName = "SeatHold";

    /// <summary>Redis TTL for temporary holds.</summary>
    public int HoldDurationMinutes { get; set; } = 10;
}
