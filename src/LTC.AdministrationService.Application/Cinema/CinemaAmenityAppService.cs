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
using LTC.AdministrationService.Entities;

namespace LTC.AdministrationService.Cinema
{
    public class CinemaAmenityAppService : ApplicationService, IAdminCinemaAmenityAppService
    {
        private readonly IRepository<CinemaAmenity, Guid> _amenityRepository;

        public CinemaAmenityAppService(IRepository<CinemaAmenity, Guid> amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<PagedResultDto<CinemaAmenityOutputDto>> GetListAsync(Guid cinemaId, GetCinemaAmenityListInputDto input)
        {
            var queryable = await _amenityRepository.GetQueryableAsync();
            queryable = queryable.Where(x => x.CinemaId == cinemaId);

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryable = queryable.Where(x => x.Name.Contains(input.Keyword) || (x.Description != null && x.Description.Contains(input.Keyword)));
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderBy(x => x.Name)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            return new PagedResultDto<CinemaAmenityOutputDto>(
                totalCount,
                items.Select(x => ObjectMapper.Map<CinemaAmenity, CinemaAmenityOutputDto>(x)).ToList()
            );
        }

        public async Task<CinemaAmenityOutputDto> GetAsync(Guid cinemaId, Guid id)
        {
            var amenity = await _amenityRepository.GetAsync(x => x.CinemaId == cinemaId && x.Id == id);
            return ObjectMapper.Map<CinemaAmenity, CinemaAmenityOutputDto>(amenity);
        }

        public async Task<CinemaAmenityOutputDto> CreateAsync(Guid cinemaId, CreateCinemaAmenityInputDto input)
        {
            var amenity = ObjectMapper.Map<CreateCinemaAmenityInputDto, CinemaAmenity>(input);
            amenity.CinemaId = cinemaId;

            amenity = await _amenityRepository.InsertAsync(amenity, autoSave: true);
            return ObjectMapper.Map<CinemaAmenity, CinemaAmenityOutputDto>(amenity);
        }

        public async Task<CinemaAmenityOutputDto> UpdateAsync(Guid cinemaId, Guid id, UpdateCinemaAmenityInputDto input)
        {
            var amenity = await _amenityRepository.GetAsync(x => x.Id == id && x.CinemaId == cinemaId);
            ObjectMapper.Map(input, amenity);

            amenity = await _amenityRepository.UpdateAsync(amenity, autoSave: true);
            return ObjectMapper.Map<CinemaAmenity, CinemaAmenityOutputDto>(amenity);
        }

        public async Task DeleteAsync(Guid cinemaId, Guid id)
        {
            await _amenityRepository.DeleteAsync(x => x.Id == id && x.CinemaId == cinemaId, autoSave: true);
        }
    }
}