using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LTC.AdministrationService.Entities;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.NewsAndOffers;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;

namespace LTC.AdministrationService.Admin
{
        public class NewsAndOffersAppService : ApplicationService, INewsAndOffersAppService
        {
            private readonly IRepository<Entities.NewsAndOffers, Guid> _newsAndOffersRepository;

            public NewsAndOffersAppService(IRepository<Entities.NewsAndOffers, Guid> newsAndOffersRepository)
            {
                _newsAndOffersRepository = newsAndOffersRepository;
            }

            public async Task<PagedResultDto<NewsAndOffersOutputDto>> GetListAsync(GetNewsAndOffersListinputDto input)
            {
                var queryable = await _newsAndOffersRepository.GetQueryableAsync();

                if (!string.IsNullOrWhiteSpace(input.Keyword))
                {
                    queryable = queryable.Where(x => x.Title.Contains(input.Keyword));
                }

                if (!string.IsNullOrWhiteSpace(input.Status))
                {
                    if (input.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    {
                        queryable = queryable.Where(x => x.IsActive);
                    }
                    else if (input.Status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        queryable = queryable.Where(x => !x.IsActive);
                    }
                }

                var totalCount = await queryable.CountAsync();
                var items = await queryable
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((input.Page - 1) * input.Fetch)
                    .Take(input.Fetch)
                    .ToListAsync();

                return new PagedResultDto<NewsAndOffersOutputDto>(
                    totalCount,
                    items.Select(x => ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(x)).ToList()
                );
            }

            public async Task<NewsAndOffersOutputDto> GetAsync(Guid id)
            {
                var newsAndOffer = await _newsAndOffersRepository.GetAsync(id);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(newsAndOffer);
            }

            public async Task<NewsAndOffersOutputDto> CreateAsync(CreateNewsAndOffersDto input)
            {
                var entity = new Entities.NewsAndOffers(GuidGenerator.Create())
                {
                    CinemaId = input.CinemaId,
                    Title = input.Title,
                    Content = input.Content,
                    StartDate = input.StartDate,
                    EndDate = input.EndDate,
                    IsActive = input.IsActive,
                    PosterUrl = input.PosterUrl ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _newsAndOffersRepository.InsertAsync(entity, true);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(entity);
            }

            public async Task<NewsAndOffersOutputDto> UpdateAsync(Guid id, UpdateNewsAndOffersDto input)
            {
                var entity = await _newsAndOffersRepository.GetAsync(id);
                entity.CinemaId = input.CinemaId;
                entity.Title = input.Title;
                entity.Content = input.Content;
                entity.StartDate = input.StartDate;
                entity.EndDate = input.EndDate;
                entity.IsActive = input.IsActive;
                entity.PosterUrl = input.PosterUrl ?? string.Empty;
                entity.UpdatedAt = DateTime.UtcNow;

                await _newsAndOffersRepository.UpdateAsync(entity, true);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(entity);
            }

            public async Task<NewsAndOffersOutputDto> DeleteAsync(Guid id)
            {
                var entity = await _newsAndOffersRepository.GetAsync(id);
                await _newsAndOffersRepository.DeleteAsync(id, true);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(entity);
            }
        }
}
