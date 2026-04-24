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
        private readonly IRepository<MovieProjection, Guid> _movieProjectionRepository;
        private readonly IRepository<MovieDistributionProjection, Guid> _distributionProjectionRepository;

        public ShowtimeAppService(
            IRepository<Showtime, Guid> repository,
            IRepository<Screen, Guid> screenRepository,
            IRepository<Cinema, Guid> cinemaRepository,
            IRepository<MovieProjection, Guid> movieProjectionRepository,
            IRepository<MovieDistributionProjection, Guid> distributionProjectionRepository)
        {
            _repository = repository;
            _screenRepository = screenRepository;
            _cinemaRepository = cinemaRepository;
            _movieProjectionRepository = movieProjectionRepository;
            _distributionProjectionRepository = distributionProjectionRepository;
        }

        public async Task<PagedResultDto<ShowtimeOutputDto>> GetListAsync(Guid movieId, Guid? cinemaId, int skipCount, int maxResultCount)
        {
            var query = await _repository.GetQueryableAsync();

            query = query.Where(x => x.MovieId == movieId);
            if (cinemaId.HasValue)
            {
                query = query.Where(x => x.CinemaId == cinemaId.Value);
            }

            var total = await query.CountAsync();
            var items = await query.Skip(skipCount).Take(maxResultCount).ToListAsync();

            return new PagedResultDto<ShowtimeOutputDto>(
                total,
                items.Select(x => ObjectMapper.Map<Showtime, ShowtimeOutputDto>(x)).ToList()
            );
        }

        public async Task<ShowtimeOutputDto> GetAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);
            return ObjectMapper.Map<Showtime, ShowtimeOutputDto>(entity);
        }

        public async Task<ShowtimeOutputDto> CreateAsync(CreateShowtimeDto input)
        {
            await ValidateCreateOrUpdateAsync(input, null);

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
                BasePrice = input.BasePrice,
                Status = input.Status ?? "Scheduled"
            };

            await _repository.InsertAsync(entity, true);
            return ObjectMapper.Map<Showtime, ShowtimeOutputDto>(entity);
        }

        public async Task<ShowtimeOutputDto> UpdateAsync(Guid id, CreateShowtimeDto input)
        {
            await ValidateCreateOrUpdateAsync(input, id);

            var entity = await _repository.GetAsync(id);
            entity.MovieId = input.MovieId;
            entity.CinemaId = input.CinemaId;
            entity.DistributionId = input.DistributionId;
            entity.MovieFormat = input.MovieFormat;
            entity.ScreenId = input.ScreenId;
            entity.ShowDate = input.ShowDate;
            entity.StartTime = input.StartTime;
            entity.EndTime = input.EndTime;
            entity.BasePrice = input.BasePrice;
            entity.Status = input.Status ?? entity.Status;
            entity.UpdatedAt = Clock.Now;

            await _repository.UpdateAsync(entity, true);
            return ObjectMapper.Map<Showtime, ShowtimeOutputDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        private async Task ValidateCreateOrUpdateAsync(CreateShowtimeDto input, Guid? editingId)
        {
            var movie = await _movieProjectionRepository.FindAsync(input.MovieId);
            if (movie == null)
            {
                throw new BusinessException("Showtime:MovieNotFound");
            }

            var normalizedStatus = (movie.Status ?? string.Empty).Trim().ToLowerInvariant();
            if (normalizedStatus != "now_showing" && normalizedStatus != "nowshowing")
            {
                throw new BusinessException("Showtime:MovieStatusInvalid");
            }

            var distribution = await _distributionProjectionRepository.FindAsync(input.DistributionId);
            if (distribution == null || distribution.MovieId != input.MovieId)
            {
                throw new BusinessException("Showtime:DistributionNotFoundForMovie");
            }

            var showDate = input.ShowDate.Date;
            if (showDate < distribution.StartDate.Date || showDate > distribution.EndDate.Date)
            {
                throw new BusinessException("Showtime:ShowDateOutOfDistributionRange");
            }

            var screen = await _screenRepository.FindAsync(input.ScreenId);
            if (screen == null)
            {
                throw new BusinessException("Showtime:ScreenNotFound");
            }
            if (!string.Equals(screen.Status, "active", StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("Showtime:ScreenNotActive");
            }
            if (screen.CinemaId != input.CinemaId)
            {
                throw new BusinessException("Showtime:ScreenCinemaMismatch");
            }

            var cinema = await _cinemaRepository.FindAsync(input.CinemaId);
            if (cinema == null)
            {
                throw new BusinessException("Showtime:CinemaNotFound");
            }
            if (CurrentUser.Id.HasValue && cinema.ManagerUserId.HasValue && cinema.ManagerUserId != CurrentUser.Id)
            {
                throw new BusinessException("Showtime:InvalidManagerCinema");
            }

            if (input.BasePrice <= 0)
            {
                throw new BusinessException("Showtime:BasePriceInvalid");
            }

            // Business timezone is GMT+7 and must be between 06:00 and 24:00.
            if (input.StartTime < TimeSpan.FromHours(6) || input.StartTime >= TimeSpan.FromHours(24))
            {
                throw new BusinessException("Showtime:StartTimeOutOfAllowedRange");
            }

            var movieFormatValue = ValidateMovieFormatJson(input.MovieFormat);

            if (!string.Equals(movieFormatValue, screen.ScreenType, StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("Showtime:MovieFormatNotSupportedByScreen");
            }

            var minDurationMinutes = movie.DurationInMinutes + 10;
            var actualDurationMinutes = (input.EndTime - input.StartTime).TotalMinutes;
            if (actualDurationMinutes < minDurationMinutes)
            {
                throw new BusinessException("Showtime:DurationTooShort");
            }

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
                    throw new BusinessException("Showtime:ScreenScheduleOverlaps");
                }

                // Gap rule: at least 5 minutes between adjacent showtimes on same screen
                var inputStartsAfterExisting = input.StartTime >= existing.EndTime;
                if (inputStartsAfterExisting)
                {
                    var gap = input.StartTime - existing.EndTime;
                    if (gap.TotalMinutes < 5)
                    {
                        throw new BusinessException("Showtime:InsufficientGapBetweenShowtimes");
                    }
                }
                else
                {
                    var gap = existing.StartTime - input.EndTime;
                    if (gap.TotalMinutes < 5)
                    {
                        throw new BusinessException("Showtime:InsufficientGapBetweenShowtimes");
                    }
                }
            }
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