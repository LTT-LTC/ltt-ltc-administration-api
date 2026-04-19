using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers;

namespace LTC.AdministrationService.Controllers
{
    [RemoteService]
    [Area("administration")]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/cinema/{cinemaId}/product")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin")]
    public class ProductController : AdministrationServiceController
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetListAsync(Guid cinemaId) => Ok();
        
        [HttpPost]
        public virtual async Task<IActionResult> CreateAsync(Guid cinemaId) => Ok();
        
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> UpdateAsync(Guid cinemaId, Guid id) => Ok();
        
        // Variants
        [HttpPost("{productId}/variants")]
        public virtual async Task<IActionResult> CreateVariantAsync(Guid cinemaId, Guid productId) => Ok();
        
        // Categories
        [HttpGet("categories")]
        public virtual async Task<IActionResult> GetCategoriesAsync(Guid cinemaId) => Ok();
        
        // Amenities (Read Only for manager, assigned by admin)
        [HttpGet("amenities")]
        public virtual async Task<IActionResult> GetAmenitiesAsync(Guid cinemaId) => Ok();
    }
}
