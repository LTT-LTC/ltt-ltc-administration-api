using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class NewsAndOffers : Entity<Guid>, IMultiTenant
    {
        public NewsAndOffers()
        {
        }

        public NewsAndOffers(Guid id) : base(id)
        {
        }

        public Guid? TenantId { get; set; }
        public Guid? CinemaId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? PosterUrl { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
