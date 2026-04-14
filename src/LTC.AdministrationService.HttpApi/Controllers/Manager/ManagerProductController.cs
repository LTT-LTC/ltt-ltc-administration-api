using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers;

namespace LTC.AdministrationService.HttpApi.Controllers.Manager
{
    [RemoteService]
    [Area("administration")]
    [Route("ltc/administration-service/api/administration/manager/cinemas/{cinemaId}")]
    [ApiController]
    public class ManagerProductController : AdministrationServiceController
    {
        [HttpGet("products")]
        public virtual async Task<IActionResult> GetListAsync(Guid cinemaId) => Ok();
        
        [HttpPost("products")]
        public virtual async Task<IActionResult> CreateAsync(Guid cinemaId) => Ok();
        
        [HttpPut("products/{id}")]
        public virtual async Task<IActionResult> UpdateAsync(Guid cinemaId, Guid id) => Ok();
        
        // Variants
        [HttpPost("products/{productId}/variants")]
        public virtual async Task<IActionResult> CreateVariantAsync(Guid cinemaId, Guid productId) => Ok();
        
        // Categories
        [HttpGet("product-categories")]
        public virtual async Task<IActionResult> GetCategoriesAsync(Guid cinemaId) => Ok();
        
        // Amenities (Read Only for manager, assigned by admin)
        [HttpGet("amenities")]
        public virtual async Task<IActionResult> GetAmenitiesAsync(Guid cinemaId) => Ok();
    }
}
