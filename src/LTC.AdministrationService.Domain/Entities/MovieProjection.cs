using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class MovieProjection : Entity<Guid>, IMultiTenant
    {
        public MovieProjection()
        {
        }

        public MovieProjection(Guid id) : base(id)
        {
        }

        public Guid? TenantId { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public int DurationInMinutes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
