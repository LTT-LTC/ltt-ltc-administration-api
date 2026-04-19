using System;

namespace LTC.AdministrationService.Admin.SeatTypes.Dtos.Output
{
    public class SeatTypeOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int NumberOfSeat { get; set; }
        public string? DisplayDirection { get; set; }
        public decimal PriceMultiplier { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}