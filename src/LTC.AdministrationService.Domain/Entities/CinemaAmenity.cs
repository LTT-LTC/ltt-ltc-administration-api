using System;
using Volo.Abp.Domain.Entities;

namespace LTC.AdministrationService.Entities
{
    public class CinemaAmenity : Entity<Guid>
    {
        public Guid CinemaId { get; set; }
        public Guid AmenitiesTypeId { get; set; }
    }
}
