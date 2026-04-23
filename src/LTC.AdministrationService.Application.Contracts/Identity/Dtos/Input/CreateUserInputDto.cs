using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace LTC.AdministrationService.Identity.Dtos.Input
{
    public class CreateUserInputDto : IdentityUserCreateOrUpdateDtoBase
    {
        public List<string> Roles { get; set; } = [];
    }
}
