using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace LTC.AdministrationService
{
    public class FileUploadedOutputDto
    {
        public string ResourceType { get; set; }
        public string SecureUrl { get; set; }
        public string PublicId { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string AssetId { get; set; }
        public string Format { get; set; }
        public long Size { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
