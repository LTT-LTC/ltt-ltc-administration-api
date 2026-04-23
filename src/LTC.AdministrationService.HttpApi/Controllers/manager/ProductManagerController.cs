using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager Product operations: read-only access and category/amenity view.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/cinema/{cinemaId}/product")]
    public class ProductManagerController : ManagerControllerBase
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetListAsync(Guid cinemaId) => Ok();

        [HttpGet("categories")]
        public virtual async Task<IActionResult> GetCategoriesAsync(Guid cinemaId) => Ok();

        [HttpGet("amenities")]
        public virtual async Task<IActionResult> GetAmenitiesAsync(Guid cinemaId) => Ok();
    }
}
