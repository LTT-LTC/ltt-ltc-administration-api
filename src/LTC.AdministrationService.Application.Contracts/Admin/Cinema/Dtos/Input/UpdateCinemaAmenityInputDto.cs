using System;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.Cinemas.Dtos.Input
{
    public class UpdateCinemaAmenityInputDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Guid AmenitiesTypeId { get; set; }
        public Guid? ProductId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
