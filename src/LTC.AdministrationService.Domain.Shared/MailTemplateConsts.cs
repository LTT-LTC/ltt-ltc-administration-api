using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService
{
    public static class MailTemplateConsts
    {
        public static string SEND_OTP_CODE = "SEND_OTP";
        public static string PASSWORD_RECOVERY = "PASSWORD_RECOVERY";
        public static string PASSWORD_RECOVERY_SUCCESS = "PASSWORD_RECOVERY_SUCCESS";

        public static string User = "User";
        public static string OTP = "OTP";
        public static string Name = "Name";
        public static string OTPCode = "OTPCode";
        public static string ExpireMinutes = "ExpireMinutes";
        public static string TenantName = "TenantName";
        
        public static string PasswordReset = "PasswordReset";
        public static string UserName = "UserName";
        public static string ResetLink = "ResetLink";
        public static string ExpireHours = "ExpireHours";
        public static string Time = "Time";
        public static string Date = "Date";
    }
}
