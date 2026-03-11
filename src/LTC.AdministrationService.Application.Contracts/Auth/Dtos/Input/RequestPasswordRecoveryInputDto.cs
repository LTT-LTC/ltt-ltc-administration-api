using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Auth.Dtos.Input
{
    public class RequestPasswordRecoveryInputDto
    {
        public string UserName { get; set; }
    }
}
