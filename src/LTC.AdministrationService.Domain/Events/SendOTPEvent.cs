using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Events
{
    public class SendOTPEvent
    {
        public Guid UserId { get; set; }
        public int ExpireSeconds { get; set; }
    }
}
