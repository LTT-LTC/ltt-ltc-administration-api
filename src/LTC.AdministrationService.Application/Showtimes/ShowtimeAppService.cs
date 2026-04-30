using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
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
        private readonly IRepository<Screen, Guid> _screenRepository;
        private readonly IRepository<Cinema, Guid> _cinemaRepository;

        public ShowtimeAppService(
            IRepository<Showtime, Guid> repository,
            IRepository<Screen, Guid> screenRepository,
            IRepository<Cinema, Guid> cinemaRepository)
        {
            _repository = repository;
            _screenRepository = screenRepository;
            _cinemaRepository = cinemaRepository;
        }

        public async Task<PagedResultDto<ShowtimeOutputDto>> GetShowtimeListAsync(Guid movieId, Guid? cinemaId, int skipCount, int maxResultCount)
        {
            var query = await _repository.GetQueryableAsync();

            query = query.Where(x => x.MovieId == movieId);
            if (cinemaId.HasValue)
            {
                query = query.Where(x => x.CinemaId == cinemaId.Value);
            }

            var total = await query.CountAsync();
            var items = await query.Skip(skipCount).Take(maxResultCount).ToListAsync();
            var mappedItems = new List<ShowtimeOutputDto>(items.Count);
            foreach (var item in items)
            {
                mappedItems.Add(await MapToOutputDtoAsync(item));
            }

            return new PagedResultDto<ShowtimeOutputDto>(
                total,
                mappedItems
            );
        }

        public async Task<ShowtimeOutputDto> GetShowtimeAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);
            return await MapToOutputDtoAsync(entity);
        }

        public async Task<ShowtimeOutputDto> CreateShowtimeAsync(CreateShowtimeDto input)
        {
            var duration = await ValidateCreateOrUpdateAsync(input, null);

            var entity = new Showtime(GuidGenerator.Create())
            {
                MovieId = input.MovieId,
                CinemaId = input.CinemaId,
                DistributionId = input.DistributionId,
                MovieFormat = input.MovieFormat,
                ScreenId = input.ScreenId,
                ShowDate = input.ShowDate,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Duration = duration,
                BasePrice = input.BasePrice,
                Status = input.Status ?? "Scheduled",
                CreatedAt = Clock.Now,
                UpdatedAt = Clock.Now
            };

            await _repository.InsertAsync(entity, true);
            return await MapToOutputDtoAsync(entity);
        }

        public async Task<ShowtimeOutputDto> UpdateShowtimeAsync(Guid id, CreateShowtimeDto input)
        {
            var duration = await ValidateCreateOrUpdateAsync(input, id);

            var entity = await _repository.GetAsync(id);
            entity.MovieId = input.MovieId;
            entity.CinemaId = input.CinemaId;
            entity.DistributionId = input.DistributionId;
            entity.MovieFormat = input.MovieFormat;
            entity.ScreenId = input.ScreenId;
            entity.ShowDate = input.ShowDate;
            entity.StartTime = input.StartTime;
            entity.EndTime = input.EndTime;
            entity.Duration = duration;
            entity.BasePrice = input.BasePrice;
            entity.Status = input.Status ?? entity.Status;
            entity.UpdatedAt = Clock.Now;

            await _repository.UpdateAsync(entity, true);
            return await MapToOutputDtoAsync(entity);
        }

        private async Task<ShowtimeOutputDto> MapToOutputDtoAsync(Showtime entity)
        {
            var mapped = ObjectMapper.Map<Showtime, ShowtimeOutputDto>(entity);

            if (entity.Duration > 0)
            {
                mapped.Duration = entity.Duration;
                return mapped;
            }

            var computedDuration = (int)Math.Round((entity.EndTime - entity.StartTime).TotalMinutes);
            mapped.Duration = computedDuration > 0 ? computedDuration : 0;
            return mapped;
        }

        public async Task DeleteShowtimeAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        private async Task<int> ValidateCreateOrUpdateAsync(CreateShowtimeDto input, Guid? editingId)
        {
            var showDate = input.ShowDate.Date;

            var screen = await _screenRepository.FindAsync(input.ScreenId);
            if (screen == null)
            {
                throw new UserFriendlyException("Screen not found.");
            }
            if (!string.Equals(screen.Status, "active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException("Selected screen is not active.");
            }
            if (screen.CinemaId != input.CinemaId)
            {
                throw new UserFriendlyException("Selected screen does not belong to the selected cinema.");
            }

            var cinema = await _cinemaRepository.FindAsync(input.CinemaId);
            if (cinema == null)
            {
                throw new UserFriendlyException("Cinema not found.");
            }
            if (CurrentUser.Id.HasValue && cinema.ManagerUserId.HasValue && cinema.ManagerUserId != CurrentUser.Id)
            {
                throw new UserFriendlyException("You can only manage showtimes in your assigned cinema.");
            }

            if (input.BasePrice <= 0)
            {
                throw new UserFriendlyException("Base price must be greater than zero.");
            }

            // Business timezone is GMT+7 and must be between 06:00 and 24:00.
            if (input.StartTime < TimeSpan.FromHours(6) || input.StartTime >= TimeSpan.FromHours(24))
            {
                throw new UserFriendlyException("Start time must be between 06:00 and 24:00.");
            }

            var movieFormatValue = ValidateMovieFormatJson(input.MovieFormat);

            if (!string.Equals(movieFormatValue, screen.ScreenType, StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException("Selected movie format is not supported by this screen.");
            }

            // Validate runtime directly from provided showtime range.
            var actualDurationMinutes = (input.EndTime - input.StartTime).TotalMinutes;
            if (actualDurationMinutes <= 0)
            {
                throw new UserFriendlyException("End time must be after start time.");
            }

            // Keep minimum practical length to prevent accidental ultra-short slots.
            if (actualDurationMinutes < 30)
            {
                throw new UserFriendlyException("Showtime duration must be at least 30 minutes.");
            }

            // Persist duration directly in showtime (minutes).
            var effectiveDuration = (int)Math.Round(actualDurationMinutes);

            var query = await _repository.GetQueryableAsync();
            var sameScreenSameDate = await query
                .Where(x => x.ScreenId == input.ScreenId && x.ShowDate.Date == showDate)
                .Where(x => !editingId.HasValue || x.Id != editingId.Value)
                .ToListAsync();

            foreach (var existing in sameScreenSameDate)
            {
                // Overlap rule
                var overlaps = input.StartTime < existing.EndTime && existing.StartTime < input.EndTime;
                if (overlaps)
                {
                    throw new UserFriendlyException(
                        $"Screen schedule overlaps. Existing showtime on this screen is {existing.StartTime:hh\\:mm}-{existing.EndTime:hh\\:mm}.");
                }

                // Gap rule: at least 5 minutes between adjacent showtimes on same screen
                var inputStartsAfterExisting = input.StartTime >= existing.EndTime;
                if (inputStartsAfterExisting)
                {
                    var gap = input.StartTime - existing.EndTime;
                    if (gap.TotalMinutes < 5)
                    {
                        throw new UserFriendlyException("At least 5 minutes gap is required between adjacent showtimes.");
                    }
                }
                else
                {
                    var gap = existing.StartTime - input.EndTime;
                    if (gap.TotalMinutes < 5)
                    {
                        throw new UserFriendlyException("At least 5 minutes gap is required between adjacent showtimes.");
                    }
                }
            }

            return effectiveDuration;
        }

        private static string ValidateMovieFormatJson(string? movieFormatJson)
        {
            if (string.IsNullOrWhiteSpace(movieFormatJson))
            {
                throw new BusinessException("Showtime:MovieFormatRequired");
            }

            try
            {
                using var document = JsonDocument.Parse(movieFormatJson);
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    throw new BusinessException("Showtime:MovieFormatInvalidJson");
                }

                if (!root.TryGetProperty("movie_format", out var movieFormat) || movieFormat.ValueKind != JsonValueKind.String)
                {
                    throw new BusinessException("Showtime:MovieFormatValueRequired");
                }

                if (!root.TryGetProperty("movie_language", out var movieLanguage) || movieLanguage.ValueKind != JsonValueKind.String)
                {
                    throw new BusinessException("Showtime:MovieLanguageRequired");
                }

                if (!root.TryGetProperty("movie_caption", out var movieCaption) || movieCaption.ValueKind != JsonValueKind.String)
                {
                    throw new BusinessException("Showtime:MovieCaptionRequired");
                }

                return movieFormat.GetString() ?? string.Empty;
            }
            catch (JsonException)
            {
                throw new BusinessException("Showtime:MovieFormatInvalidJson");
            }
        }
    }
}