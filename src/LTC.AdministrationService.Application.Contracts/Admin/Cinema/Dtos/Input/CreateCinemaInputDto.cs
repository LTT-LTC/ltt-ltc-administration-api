using System;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.Cinema.Dtos.Input
{
    public class CreateCinemaInputDto
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        
        [MaxLength(128)]
        public string? City { get; set; }
        
        [MaxLength(128)]
        public string? Ward { get; set; }
        
        [MaxLength(512)]
        public string? Address { get; set; }
        
        public Guid? ManagerUserId { get; set; }
        
        [MaxLength(32)]
        public string? ServiceNumber { get; set; }
        
        [MaxLength(64)]
        public string? Status { get; set; }
    }
}
