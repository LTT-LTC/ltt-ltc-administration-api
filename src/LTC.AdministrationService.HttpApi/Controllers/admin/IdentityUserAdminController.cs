using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only IdentityUser operations.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/users")]
    public class IdentityUserAdminController : AdminControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
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
