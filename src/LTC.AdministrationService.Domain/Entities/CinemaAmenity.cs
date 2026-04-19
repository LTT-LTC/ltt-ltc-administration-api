using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class CinemaAmenity : Entity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid AmenitiesTypeId { get; set; }
        public Guid? ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
    }
}
