using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.SeatType.Dtos.Input
{
    public class CreateSeatTypeInputDto
    {
        [Required]
        [MaxLength(128)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(512)]
        public string? Description { get; set; }
        
        public int NumberOfSeat { get; set; }
        
        [MaxLength(64)]
        public string? DisplayDirection { get; set; }
        
        public decimal PriceMultiplier { get; set; }
    }
}