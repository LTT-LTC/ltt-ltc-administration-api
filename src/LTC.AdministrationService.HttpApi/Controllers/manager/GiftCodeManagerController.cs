using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager GiftCode operations: read, create, and update (no delete).
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/gift-codes")]
    public class GiftCodeManagerController : ManagerControllerBase
    {
        private readonly IGiftCodeAppService _giftCodeAppService;

        public GiftCodeManagerController(IGiftCodeAppService giftCodeAppService)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateGiftCodeDto input)
        {
            var result = await _giftCodeAppService.UpdateAsync(id, input);
            return Ok(result);
        }
    }
}
