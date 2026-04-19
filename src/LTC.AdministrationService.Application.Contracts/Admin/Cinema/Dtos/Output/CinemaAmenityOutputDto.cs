using System;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Admin.Cinema.Dtos.Output
{
    public class CinemaAmenityOutputDto : EntityDto<Guid>
    {
        public Guid CinemaId { get; set; }
        public Guid AmenitiesTypeId { get; set; }
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
