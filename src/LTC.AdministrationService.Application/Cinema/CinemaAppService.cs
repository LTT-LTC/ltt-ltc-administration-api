using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Admin.Cinema;
using LTC.AdministrationService.Admin.Cinema.Dtos.Input;
using LTC.AdministrationService.Admin.Cinema.Dtos.Output;

namespace LTC.AdministrationService.Cinema
{
    public class CinemaAppService : ApplicationService, IAdminCinemaAppService
    {
        private readonly IRepository<Entities.Cinema, Guid> _cinemaRepository;

        public CinemaAppService(IRepository<Entities.Cinema, Guid> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<PagedResultDto<CinemaOutputDto>> GetListAsync(GetCinemaListInputDto input)
        {
            var queryable = await _cinemaRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryable = queryable.Where(x => x.Name.Contains(input.Keyword) || (x.City != null && x.City.Contains(input.Keyword)));
            }

            if (!string.IsNullOrWhiteSpace(input.Status))
            {
                queryable = queryable.Where(x => x.Status == input.Status);
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderBy(x => x.Name)
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToListAsync();

            return new PagedResultDto<CinemaOutputDto>(
                totalCount,
                items.Select(x => ObjectMapper.Map<Entities.Cinema, CinemaOutputDto>(x)).ToList()
            );
        }

        public async Task<CinemaOutputDto> GetAsync(Guid id)
        {
            var cinema = await _cinemaRepository.GetAsync(id);
            return ObjectMapper.Map<Entities.Cinema, CinemaOutputDto>(cinema);
        }

        public async Task<CinemaOutputDto> CreateAsync(CreateCinemaInputDto input)
        {
            var cinema = ObjectMapper.Map<CreateCinemaInputDto, Entities.Cinema>(input);
            cinema.CreatedAt = DateTime.UtcNow;

            cinema = await _cinemaRepository.InsertAsync(cinema, autoSave: true);
            return ObjectMapper.Map<Entities.Cinema, CinemaOutputDto>(cinema);
        }

        public async Task<CinemaOutputDto> UpdateAsync(Guid id, UpdateCinemaInputDto input)
        {
            var cinema = await _cinemaRepository.GetAsync(id);
            ObjectMapper.Map(input, cinema);
            cinema.UpdatedAt = DateTime.UtcNow;

            cinema = await _cinemaRepository.UpdateAsync(cinema, autoSave: true);
            return ObjectMapper.Map<Entities.Cinema, CinemaOutputDto>(cinema);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _cinemaRepository.DeleteAsync(id, autoSave: true);
        }
    }
}