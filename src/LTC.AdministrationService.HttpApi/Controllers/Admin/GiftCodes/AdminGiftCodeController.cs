using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;

namespace LTC.AdministrationService.Controllers.Admin.GiftCodes
{
    [RemoteService]
    [Area("administration")]
    [Route("ltc/administration-service/api/administration/admin/gift-codes")]
    public class AdminGiftCodeController : AdministrationServiceController
    {
        private readonly IGiftCodeAppService _giftCodeAppService;

        public AdminGiftCodeController(IGiftCodeAppService giftCodeAppService)
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