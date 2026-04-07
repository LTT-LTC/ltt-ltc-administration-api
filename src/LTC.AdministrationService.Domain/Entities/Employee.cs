using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class Employee : FullAuditedEntity<Guid>, IMultiTenant
    {
        public string? EmployeeId { get; set; }
        public Guid? TenantId { get; private set; }
        public Guid? UserId { get; set; }
        public Guid? CinemaId { get; set; }
        public string? Scope { get; set; }
        public string? Position { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Status { get; set; }
        public Guid? ManagedByEmployeeId { get; set; }
        public Guid? CreatedByUserId { get; set; }

        // Restored fields for compatibility with existing AppServices
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? OtherEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? OrganizationUnitId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? AvatarFileId { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsFirstLogin { get; set; } = true;
    }
}
