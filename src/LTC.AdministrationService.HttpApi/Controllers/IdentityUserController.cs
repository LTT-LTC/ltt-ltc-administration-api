using LTC.Shared.Hosting.Microservices.HttpApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers
{
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/users")]
    [Authorize(Roles = "Admin")]
    public class IdentityUserController : AdministrationServiceController
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

            return Ok(result);
        }
    }
}
