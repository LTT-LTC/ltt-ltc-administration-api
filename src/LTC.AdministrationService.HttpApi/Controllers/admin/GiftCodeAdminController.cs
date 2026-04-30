using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only GiftCode operations: all CRUD including delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/gift-codes")]
    public class GiftCodeAdminController : AdminControllerBase
    {
        private readonly IGiftCodeAppService _giftCodeAppService;

        public GiftCodeAdminController(IGiftCodeAppService giftCodeAppService)
        {
            _giftCodeAppService = giftCodeAppService;
        }

        [HttpGet]
        public async Task<PagedResultDto<GiftCodeOutputDto>> GetGiftCodeListAsync(int skipCount = 0, int maxResultCount = 10)
        {
            return await _giftCodeAppService.GetGiftCodeListAsync(skipCount, maxResultCount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGiftCodeAsync([FromBody] CreateGiftCodeDto input)
        {
            var result = await _giftCodeAppService.CreateGiftCodeAsync(input);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGiftCodeAsync(Guid id, [FromBody] CreateGiftCodeDto input)
        {
            var result = await _giftCodeAppService.UpdateGiftCodeAsync(id, input);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGiftCodeAsync(Guid id)
        {
            await _giftCodeAppService.DeleteGiftCodeAsync(id);
            return Ok();
        }
    }
}
