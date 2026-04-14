using System;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.Screen.Dtos.Input
{
    public class UpdateScreenInputDto
    {
        public int ScreenNumber { get; set; }
        
        [MaxLength(64)]
        public string? ScreenType { get; set; }
        
        public string? SeatLayout { get; set; }
        
        public int SeatCount { get; set; }
        
        [MaxLength(64)]
        public string? Status { get; set; }
    }
}
