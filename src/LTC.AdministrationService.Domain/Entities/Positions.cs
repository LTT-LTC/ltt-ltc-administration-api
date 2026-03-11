using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    /// <summary>
    /// Chức vụ chi tiết trong từng phòng ban, ví dụ: "Trưởng phòng", "Nhân viên bán hàng", "Kế toán viên", v.v.
    /// </summary>
    public class Positions : FullAuditedEntity<Guid>, IMultiTenant
    {
        public string? Name { get; set; } // Tên chức vụ
        public Guid? OrganizationUnitId { get; set; } // Phòng ban mà chức vụ này thuộc về

        [ForeignKey(nameof(OrganizationUnitId))]
        public OrganizationUnit OrganizationUnit { get; set; }
        public Guid? TenantId { get; private set; }
    }
}
