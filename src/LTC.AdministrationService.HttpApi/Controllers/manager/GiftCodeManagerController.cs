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
    /// Manager GiftCode operations: read-only.
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

    }
}
