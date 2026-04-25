using System;

namespace LTC.AdministrationService.Admin.Screens.Dtos.Output
{
    public class ScreenOutputDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public Guid CinemaId { get; set; }
        public int ScreenNumber { get; set; }
        public string? ScreenType { get; set; }
        public Guid? SeatMapId { get; set; }
        public string? SeatMapName { get; set; }
        public string? SeatMapDescription { get; set; }
        /// <summary>Populated from linked <c>SeatMap</c> for API compatibility.</summary>
        public string? SeatLayout { get; set; }
        /// <summary>Populated from linked <c>SeatMap</c> for API compatibility.</summary>
        public int SeatCount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
