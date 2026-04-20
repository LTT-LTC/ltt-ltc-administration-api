using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.GiftCodes.Dtos;

namespace LTC.AdministrationService.GiftCodes
{
    public interface IGiftCodeAppService : IApplicationService
    {
        Task<PagedResultDto<GiftCodeOutputDto>> GetListAsync(int skipCount, int maxResultCount);
        Task<GiftCodeOutputDto> CreateAsync(CreateGiftCodeDto input);
        Task<GiftCodeOutputDto> UpdateAsync(Guid id, CreateGiftCodeDto input);
        Task DeleteAsync(Guid id);
    }
}