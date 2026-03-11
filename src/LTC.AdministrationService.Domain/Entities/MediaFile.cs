using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities
{
    public class MediaFile : FullAuditedEntity<Guid>, IMultiTenant
    {
        public string DisplayName { get; set; } // Tên hiển thị của file
        public string ResourceType { get; set; } // Loại tài nguyên (ví dụ: image, video, raw)
        public string SecureUrl { get; set; } // URL an toàn để truy cập file
        public string PublicId { get; set; } // ID công khai của file trên Cloudinary
        public string Type { get; set; } // Loại file (ví dụ: upload, authenticated)
        public string AssetId { get; set; } // ID tài sản của file trên Cloudinary
        public string Format { get; set; } // Định dạng của file (ví dụ: jpg, png, mp4)
        public long Size { get; set; } // Kích thước của file (tính bằng byte)
        public Guid? TenantId { get; private set; }

        public MediaFile() { }
        public MediaFile(Guid id)
        {
            Id = id;
        }
    }
}
