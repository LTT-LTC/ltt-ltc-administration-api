using System.Collections.Generic;

namespace LTC.AdministrationService.Showtimes.Dtos;

public class MergePaidShowtimeSeatsInputDto
{
    public List<string> SeatCodes { get; set; } = new();
}
