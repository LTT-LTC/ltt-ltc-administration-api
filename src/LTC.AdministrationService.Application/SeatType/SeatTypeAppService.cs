using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Admin.SeatTypes;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Input;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Output;
using LTC.Shared.Hosting.Microservices.Timing;

namespace LTC.AdministrationService.SeatTypes
{
    public class SeatTypeAppService : ApplicationService, IAdminSeatTypeAppService
    {
        private readonly IRepository<Entities.SeatType, Guid> _seatTypeRepository;
        private readonly IGmt7Clock _gmt7Clock;

        public SeatTypeAppService(
            IRepository<Entities.SeatType, Guid> seatTypeRepository,
            IGmt7Clock gmt7Clock)
        {
            _seatTypeRepository = seatTypeRepository;
            _gmt7Clock = gmt7Clock;
        }

        public async Task<PagedResultDto<SeatTypeOutputDto>> GetSeatTypeListAsync(GetSeatTypeListInputDto input)
        {
            var queryable = await _seatTypeRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryable = queryable.Where(x => x.Name.Contains(input.Keyword));
            }

            var totalCount = await queryable.CountAsync();
            // Use long arithmetic to prevent integer overflow with large page numbers
            var skipCount = (long)(input.Page - 1) * input.Fetch;
            var items = await queryable
                .OrderBy(x => x.Name)
                .Skip((int)skipCount)
                .Take(input.Fetch)
                .ToListAsync();

            return new PagedResultDto<SeatTypeOutputDto>(
                totalCount,
                items.Select(x => ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(x)).ToList()
            );
        }

        public async Task<SeatTypeOutputDto> GetSeatTypeAsync(Guid id)
        {
            var entity = await _seatTypeRepository.GetAsync(id);
            return ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(entity);
        }

        public async Task<SeatTypeOutputDto> CreateSeatTypeAsync(CreateSeatTypeInputDto input)
        {
            var entity = ObjectMapper.Map<CreateSeatTypeInputDto, Entities.SeatType>(input);
            entity.UpdatedAt = _gmt7Clock.Gmt7Now;

            entity = await _seatTypeRepository.InsertAsync(entity, autoSave: true);
            return ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(entity);
        }

        public async Task<SeatTypeOutputDto> UpdateSeatTypeAsync(Guid id, UpdateSeatTypeInputDto input)
        {
            var entity = await _seatTypeRepository.GetAsync(id);
            ObjectMapper.Map(input, entity);
            entity.UpdatedAt = _gmt7Clock.Gmt7Now;

            entity = await _seatTypeRepository.UpdateAsync(entity, autoSave: true);
            return ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(entity);
        }

        public async Task DeleteSeatTypeAsync(Guid id)
        {
            await _seatTypeRepository.DeleteAsync(id, autoSave: true);
        }
    }
}