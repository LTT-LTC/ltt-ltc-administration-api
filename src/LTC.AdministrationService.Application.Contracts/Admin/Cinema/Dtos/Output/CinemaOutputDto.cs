using System;

namespace LTC.AdministrationService.Admin.Cinema.Dtos.Output
{
    public class CinemaOutputDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string Name { get; set; }
        public string? City { get; set; }
        public string? Ward { get; set; }
        public string? Address { get; set; }
        public Guid? ManagerUserId { get; set; }
        public string? ServiceNumber { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
