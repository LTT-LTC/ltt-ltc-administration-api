using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class MovieDistributionProjection : Entity<Guid>, IMultiTenant
    {
        public MovieDistributionProjection()
        {
        }

        public MovieDistributionProjection(Guid id) : base(id)
        {
        }

        public Guid? TenantId { get; set; }
        public Guid MovieId { get; set; }
        public string? Format { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
