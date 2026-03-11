using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class OrganizationUnit : FullAuditedEntity<Guid>, IMultiTenant
    {
        public string? DisplayName { get; set; } // Tên hiển thị của phòng ban
        public string? Code { get; set; } // Mã định danh duy nhất của phòng ban
        public string? ParentCode { get; set; } // Mã định danh của phòng ban cha (nếu có)
        public string? CodePath { get; set; } // Đường dẫn mã định danh từ gốc đến phòng ban hiện tại (ví dụ: "TDTU/SALES/DOMESTIC")
        public string? NamePath { get; set; } // Đường dẫn tên hiển thị từ gốc đến phòng ban hiện tại (ví dụ: "Công ty/Cửa hàng/Bán lẻ")
        public Guid? TenantId { get; private set; }
    }
}
