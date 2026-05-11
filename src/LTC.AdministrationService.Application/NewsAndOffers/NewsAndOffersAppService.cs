using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using LTC.AdministrationService.Entities;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using LTC.AdministrationService.NewsAndOffers;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;
using LTC.Shared.Hosting.Microservices.Timing;

namespace LTC.AdministrationService.Admin
{
        public class NewsAndOffersAppService : ApplicationService, INewsAndOffersAppService
        {
            private const string NewsAndOffersPosterFolder = "ltt-ltc/administration/news-and-offers/poster";
            private readonly IRepository<Entities.NewsAndOffers, Guid> _newsAndOffersRepository;
            private readonly IGmt7Clock _gmt7Clock;
            private readonly Cloudinary _cloudinary;

            public NewsAndOffersAppService(
                IRepository<Entities.NewsAndOffers, Guid> newsAndOffersRepository,
                IGmt7Clock gmt7Clock,
                Cloudinary cloudinary)
            {
                _newsAndOffersRepository = newsAndOffersRepository;
                _gmt7Clock = gmt7Clock;
                _cloudinary = cloudinary;
            }

            private async Task<string?> UploadPosterIfProvidedAsync(Microsoft.AspNetCore.Http.IFormFile? imageFile)
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return null;
                }

                if (CurrentUser == null || (!CurrentUser.IsInRole("admin") && !CurrentUser.IsInRole("manager")))
                {
                    throw new UserFriendlyException("Only admins and managers can perform this action.");
                }

                await using var stream = imageFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    Folder = NewsAndOffersPosterFolder,
                    PublicId = $"news_and_offers_poster_{Guid.CreateVersion7()}",
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult == null || uploadResult.Error != null || uploadResult.SecureUrl == null)
                {
                    var errorMessage = uploadResult?.Error?.Message ?? "Upload poster failed.";
                    throw new UserFriendlyException(errorMessage);
                }

                return uploadResult.SecureUrl.ToString();
            }

            public async Task<PagedResultDto<NewsAndOffersOutputDto>> GetNewsAndOffersListAsync(GetNewsAndOffersListinputDto input)
            {
                var queryable = await _newsAndOffersRepository.GetQueryableAsync();
                var currentTenantId = CurrentTenant?.Id;

                queryable = queryable.Where(x => !x.IsDeleted);

                // Scope by current tenant resolved from X-Tenant.
                if (currentTenantId.HasValue)
                {
                    queryable = queryable.Where(x => x.TenantId == currentTenantId.Value);
                }
                else
                {
                    queryable = queryable.Where(x => x.TenantId == null);
                }

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
                // Use long arithmetic to prevent integer overflow with large page numbers
                var skipCount = (long)(input.Page - 1) * input.Fetch;
                var items = await queryable
                    .OrderByDescending(x => x.StartDate ?? DateTime.MinValue)
                    .ThenByDescending(x => x.CreatedAt ?? DateTime.MinValue)
                    .Skip((int)skipCount)
                    .Take(input.Fetch)
                    .ToListAsync();

                return new PagedResultDto<NewsAndOffersOutputDto>(
                    totalCount,
                    items.Select(x => ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(x)).ToList()
                );
            }

            public async Task<NewsAndOffersOutputDto> GetNewsAndOffersAsync(Guid id)
            {
                var newsAndOffer = await _newsAndOffersRepository.GetAsync(id);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(newsAndOffer);
            }

            public async Task<NewsAndOffersOutputDto> CreateNewsAndOffersAsync(CreateNewsAndOffersDto input)
            {
                var uploadedPosterUrl = await UploadPosterIfProvidedAsync(input.ImageFile);
                var entity = new Entities.NewsAndOffers(GuidGenerator.Create())
                {
                    CinemaId = input.CinemaId,
                    Title = input.Title,
                    Content = input.Content,
                    StartDate = input.StartDate,
                    EndDate = input.EndDate,
                    IsActive = input.IsActive,
                    PosterUrl = uploadedPosterUrl ?? input.PosterUrl ?? string.Empty,
                    CreatedAt = _gmt7Clock.Gmt7Now,
                    UpdatedAt = _gmt7Clock.Gmt7Now,
                    IsDeleted = false
                };

                await _newsAndOffersRepository.InsertAsync(entity, true);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(entity);
            }

            public async Task<NewsAndOffersOutputDto> UpdateNewsAndOffersAsync(Guid id, UpdateNewsAndOffersDto input)
            {
                var entity = await _newsAndOffersRepository.GetAsync(id);
                var uploadedPosterUrl = await UploadPosterIfProvidedAsync(input.ImageFile);
                entity.CinemaId = input.CinemaId;
                entity.Title = input.Title;
                entity.Content = input.Content;
                entity.StartDate = input.StartDate;
                entity.EndDate = input.EndDate;
                entity.IsActive = input.IsActive;
                entity.PosterUrl = uploadedPosterUrl ?? input.PosterUrl ?? string.Empty;
                entity.UpdatedAt = _gmt7Clock.Gmt7Now;

                await _newsAndOffersRepository.UpdateAsync(entity, true);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(entity);
            }

            public async Task<NewsAndOffersOutputDto> DeleteNewsAndOffersAsync(Guid id)
            {
                var entity = await _newsAndOffersRepository.GetAsync(id);
                await _newsAndOffersRepository.DeleteAsync(id, true);
                return ObjectMapper.Map<Entities.NewsAndOffers, NewsAndOffersOutputDto>(entity);
            }
        }
}
