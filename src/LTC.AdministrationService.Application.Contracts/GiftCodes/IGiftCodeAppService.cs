using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.GiftCodes.Dtos;

namespace LTC.AdministrationService.GiftCodes
{
    public interface IGiftCodeAppService : IApplicationService
    {
        Task<PagedResultDto<GiftCodeOutputDto>> GetGiftCodeListAsync(int skipCount, int maxResultCount);
        Task<GiftCodeOutputDto> CreateGiftCodeAsync(CreateGiftCodeDto input);
        Task<GiftCodeOutputDto> UpdateGiftCodeAsync(Guid id, CreateGiftCodeDto input);
        Task DeleteGiftCodeAsync(Guid id);
    }
}