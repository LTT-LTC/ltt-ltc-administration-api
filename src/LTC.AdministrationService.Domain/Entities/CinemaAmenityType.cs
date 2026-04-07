using System;
using Volo.Abp.Domain.Entities;

namespace LTC.AdministrationService.Entities
{
    public class CinemaAmenityType : Entity<Guid>
    {
        public string Name { get; set; }
        public string? Icon { get; set; }
    }
}
