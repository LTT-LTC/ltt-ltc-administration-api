using System.Threading.Tasks;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Customer;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/customer/gift-codes")]
public class GiftCodeCustomerController : CustomerControllerBase
{
    private readonly IGiftCodeAppService _giftCodeAppService;

    public GiftCodeCustomerController(IGiftCodeAppService giftCodeAppService)
    {
        _giftCodeAppService = giftCodeAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<GiftCodeOutputDto>> GetGiftCodeListAsync(int skipCount = 0, int maxResultCount = 10)
        => _giftCodeAppService.GetGiftCodeListAsync(skipCount, maxResultCount);
}
