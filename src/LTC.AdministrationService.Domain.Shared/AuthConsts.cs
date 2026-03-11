using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService
{
    public static class AuthConsts
    {
        public static int OTP_EXPIRE_SECONDS = 300; // 5 minutes
        public static int PASSWORD_RESET_TOKEN_EXPIRE_HOURS = 24; // 24 hours
    }
}
