using LTC.Shared.Hosting.Microservices.HttpApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers.Identity
{
    [Route($"{AdministrationServiceSettingNames.DefaultRoute}/users")]
    public class IdentityUserController : AppControllerBase
    {
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            // Demo object trả về
            var result = new
            {
                Id = id,
                Name = "Demo User",
                Email = "demo@example.com"
            };

            return Success(result);
        }
    }
}
