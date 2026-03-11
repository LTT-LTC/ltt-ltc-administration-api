using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService
{
    public class DeleteFileInputDto
    {
        public string PublicId { get; set; }
        public string ResourceType { get; set; }
    }
}
