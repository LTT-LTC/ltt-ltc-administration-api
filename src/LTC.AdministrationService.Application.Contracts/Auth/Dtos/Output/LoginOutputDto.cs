using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Auth.Dtos.Output
{
    public class LoginOutputDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
        public bool IsOTPSent { get; set; }
        public bool IsFirstLogin { get; set; }
    }
}
