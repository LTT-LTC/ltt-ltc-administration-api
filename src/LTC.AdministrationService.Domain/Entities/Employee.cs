using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class Employee : FullAuditedEntity<Guid>, IMultiTenant
    {
        // IsDeleted => Default value (0) / convertbit(0)
        public string Name { get; set; } // Họ và tên
        public string Code { get; set; }
        public string Email { get; set; } // Email cá nhân
        public string? OtherEmail { get; set; } // Email khác
        public Guid? OrganizationUnitId { get; set; } // Phòng ban
        public Guid? PositionId { get; set; } // Chức vụ trong phòng ban
        public Guid? AvatarFileId { get; set; } // Ảnh đại diện
        public string PhoneNumber { get; set; } // Số điện thoại cá nhân
        public DateTime? JoinedDate { get; set; } // Ngày vào
        public DateTime? DateOfBirth { get; set; } // Ngày sinh
        public int NumberOfLogin { get; set; } // Số lần đăng nhập
        public DateTime? LastLoginTime { get; set; } // Thời gian đăng nhập cuối cùng
        public DateTime? NextLoginTime { get; set; } // Thời gian đăng nhập tiếp theo
        public Guid? TenantId { get; private set; }
        public Guid? UserId { get; set; } // Id của user trong IdentityUser
        public bool IsFirstLogin { get; set; } // Lần đầu đăng nhập ?

        [ForeignKey(nameof(OrganizationUnitId))]
        public OrganizationUnit OrganizationUnit { get; set; }

        [ForeignKey(nameof(PositionId))]
        public Positions Position { get; set; }
    }
}
