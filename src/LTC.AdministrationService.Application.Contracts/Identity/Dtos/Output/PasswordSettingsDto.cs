using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Identity.Dtos.Output
{
    public class PasswordSettingsDto
    {
        public bool IsEnabled { get; set; }
        public string DefaultPassword { get; set; }
    }
}
