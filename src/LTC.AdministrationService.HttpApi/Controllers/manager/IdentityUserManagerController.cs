using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager IdentityUser operations: read-only access.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/users")]
    public class IdentityUserManagerController : ManagerControllerBase
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
