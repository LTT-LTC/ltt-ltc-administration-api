using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.SeatMaps.Dtos.Input
{
    public class CreateSeatMapInputDto
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public string? SeatLayout { get; set; }

        [Range(1, int.MaxValue)]
        public int SeatCount { get; set; }
    }
}
