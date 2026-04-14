using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.GiftCodes.Dtos;

namespace LTC.AdministrationService.GiftCodes
{
    public class GiftCodeAppService : ApplicationService, IGiftCodeAppService
    {
        private readonly IRepository<GiftCode, Guid> _repository;

        public GiftCodeAppService(IRepository<GiftCode, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<GiftCodeOutputDto>> GetListAsync(int skipCount, int maxResultCount)
        {
            var query = await _repository.GetQueryableAsync();

            var total = await query.CountAsync();
            var items = await query.Skip(skipCount).Take(maxResultCount).ToListAsync();

            return new PagedResultDto<GiftCodeOutputDto>(
                total,
                items.Select(x => ObjectMapper.Map<GiftCode, GiftCodeOutputDto>(x)).ToList()
            );
        }

        public async Task<GiftCodeOutputDto> CreateAsync(CreateGiftCodeDto input)
        {
            var entity = new GiftCode(GuidGenerator.Create())
            {
                Code = input.Code,
                Description = input.Description,
                DiscountType = input.DiscountType,
                DiscountValue = input.DiscountValue,
                MinOrderAmount = input.MinOrderAmount,
                UsageLimit = input.UsageLimit,
                UsageCount = 0,
                PerUserLimit = input.PerUserLimit,
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Status = input.Status ?? "Active"
            };

            await _repository.InsertAsync(entity, true);
            return ObjectMapper.Map<GiftCode, GiftCodeOutputDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}