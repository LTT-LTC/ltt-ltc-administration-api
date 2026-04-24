using System.Threading.Tasks;
using LTC.AdministrationService.GiftCodes;
using LTC.AdministrationService.GiftCodes.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Staff;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/staff/gift-codes")]
public class GiftCodeStaffController : StaffControllerBase
{
    private readonly IGiftCodeAppService _giftCodeAppService;

    public GiftCodeStaffController(IGiftCodeAppService giftCodeAppService)
    {
        _giftCodeAppService = giftCodeAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<GiftCodeOutputDto>> GetListAsync(int skipCount = 0, int maxResultCount = 10)
        => _giftCodeAppService.GetListAsync(skipCount, maxResultCount);
}
