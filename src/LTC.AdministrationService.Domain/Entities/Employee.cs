using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class Employee : FullAuditedEntity<Guid>, IMultiTenant
    {
        [NotMapped]
        public string? EmployeeId { get; set; }
        [NotMapped]
        public bool IsFirstLogin { get; set; } = true;
        public Guid? TenantId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CinemaId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Position { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Status { get; set; }
        public Guid? ManagedByEmployeeId { get; set; }
        public Guid? CreatedByUserId { get; set; }
    }
}
