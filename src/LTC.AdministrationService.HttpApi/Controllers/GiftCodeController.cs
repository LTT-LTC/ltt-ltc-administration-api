using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;

namespace LTC.AdministrationService.Controllers
{
    [RemoteService]
    [Area("administration")]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/gift-codes")]
    [Authorize(Roles = "Admin,Manager")]
    public class GiftCodeController : AdministrationServiceController
    {
        private readonly IGiftCodeAppService _giftCodeAppService;

        public GiftCodeController(IGiftCodeAppService giftCodeAppService)
        {
            _giftCodeAppService = giftCodeAppService;
        }

        [HttpGet]
        public async Task<PagedResultDto<GiftCodeOutputDto>> GetListAsync(int skipCount = 0, int maxResultCount = 10)
        {
            return await _giftCodeAppService.GetListAsync(skipCount, maxResultCount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGiftCodeDto input)
        {
            var result = await _giftCodeAppService.CreateAsync(input);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _giftCodeAppService.DeleteAsync(id);
            return Ok();
        }
    }
}