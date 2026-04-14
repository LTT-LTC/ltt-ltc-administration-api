using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;

namespace LTC.AdministrationService.Controllers.Manager.GiftCodes
{
    [RemoteService]
    [Area("administration")]
    [Route("ltc/administration-service/api/administration/manager/gift-codes")]
    public class ManagerGiftCodeController : AdministrationServiceController
    {
        private readonly IGiftCodeAppService _giftCodeAppService;

        public ManagerGiftCodeController(IGiftCodeAppService giftCodeAppService)
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