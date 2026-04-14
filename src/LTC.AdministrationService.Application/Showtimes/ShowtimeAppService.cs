using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Showtimes.Dtos;

namespace LTC.AdministrationService.Showtimes
{
    public class ShowtimeAppService : ApplicationService, IShowtimeAppService
    {
        private readonly IRepository<Showtime, Guid> _repository;

        public ShowtimeAppService(IRepository<Showtime, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<ShowtimeOutputDto>> GetListAsync(Guid cinemaId, int skipCount, int maxResultCount)
        {
            var query = await _repository.GetQueryableAsync();

            query = query.Where(x => x.CinemaId == cinemaId);

            var total = await query.CountAsync();
            var items = await query.Skip(skipCount).Take(maxResultCount).ToListAsync();

            return new PagedResultDto<ShowtimeOutputDto>(
                total,
                items.Select(x => ObjectMapper.Map<Showtime, ShowtimeOutputDto>(x)).ToList()
            );
        }

        public async Task<ShowtimeOutputDto> CreateAsync(CreateShowtimeDto input)
        {
            var entity = new Showtime(GuidGenerator.Create())
            {
                CinemaId = input.CinemaId,
                DistributionId = input.DistributionId,
                ScreenId = input.ScreenId,
                ShowDate = input.ShowDate,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                BasePrice = input.BasePrice,
                Status = input.Status ?? "Scheduled"
            };

            await _repository.InsertAsync(entity, true);
            return ObjectMapper.Map<Showtime, ShowtimeOutputDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}