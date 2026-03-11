using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Events
{
    public class RecoveryPasswordEvent
    {
        // thêm data cần truyền vào đây (như dto)
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string ResetToken { get; set; }
        public string ResetUrl { get; set; }
        public int ExpireHours { get; set; }
    }
}
