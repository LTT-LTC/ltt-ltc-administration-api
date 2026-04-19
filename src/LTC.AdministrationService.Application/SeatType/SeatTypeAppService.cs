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

namespace LTC.AdministrationService.SeatTypes
{
    public class SeatTypeAppService : ApplicationService, IAdminSeatTypeAppService
    {
        private readonly IRepository<Entities.SeatType, Guid> _seatTypeRepository;

        public SeatTypeAppService(IRepository<Entities.SeatType, Guid> seatTypeRepository)
        {
            _seatTypeRepository = seatTypeRepository;
        }

        public async Task<PagedResultDto<SeatTypeOutputDto>> GetListAsync(GetSeatTypeListInputDto input)
        {
            var queryable = await _seatTypeRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryable = queryable.Where(x => x.Name.Contains(input.Keyword));
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderBy(x => x.Name)
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToListAsync();

            return new PagedResultDto<SeatTypeOutputDto>(
                totalCount,
                items.Select(x => ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(x)).ToList()
            );
        }

        public async Task<SeatTypeOutputDto> GetAsync(Guid id)
        {
            var entity = await _seatTypeRepository.GetAsync(id);
            return ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(entity);
        }

        public async Task<SeatTypeOutputDto> CreateAsync(CreateSeatTypeInputDto input)
        {
            var entity = ObjectMapper.Map<CreateSeatTypeInputDto, Entities.SeatType>(input);
            entity.UpdatedAt = DateTime.UtcNow;

            entity = await _seatTypeRepository.InsertAsync(entity, autoSave: true);
            return ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(entity);
        }

        public async Task<SeatTypeOutputDto> UpdateAsync(Guid id, UpdateSeatTypeInputDto input)
        {
            var entity = await _seatTypeRepository.GetAsync(id);
            ObjectMapper.Map(input, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            entity = await _seatTypeRepository.UpdateAsync(entity, autoSave: true);
            return ObjectMapper.Map<Entities.SeatType, SeatTypeOutputDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _seatTypeRepository.DeleteAsync(id, autoSave: true);
        }
    }
}