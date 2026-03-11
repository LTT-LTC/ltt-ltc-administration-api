using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService
{
    public class FileOutputDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string SecureUrl { get; set; }
        public long Size { get; set; }
        public string Format { get; set; }
        public string ResourceType { get; set; }
    }
}
