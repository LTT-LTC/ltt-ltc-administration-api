using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only Product operations: create, update, delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/cinema/{cinemaId}/product")]
    public class ProductAdminController : AdminControllerBase
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetListAsync(Guid cinemaId) => Ok();

        [HttpGet("categories")]
        public virtual async Task<IActionResult> GetCategoriesAsync(Guid cinemaId) => Ok();

        [HttpGet("amenities")]
        public virtual async Task<IActionResult> GetAmenitiesAsync(Guid cinemaId) => Ok();

        [HttpPost]
        public virtual async Task<IActionResult> CreateAsync(Guid cinemaId) => Ok();

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> UpdateAsync(Guid cinemaId, Guid id) => Ok();

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(Guid cinemaId, Guid id) => Ok();

        [HttpPost("{productId}/variants")]
        public virtual async Task<IActionResult> CreateVariantAsync(Guid cinemaId, Guid productId) => Ok();
    }
}
