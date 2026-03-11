using LTC.Shared.CrossCuttingConcerns.Enums;
using Microsoft.AspNetCore.Http;

namespace LTC.AdministrationService
{
    public class UploadFileInputDto
    {
        public IFormFile File { get; set; }
        public FileTypeEnum FileType { get; set; }
    }
}
