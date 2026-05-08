using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Input;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Output;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Movies;
using LTC.AdministrationService.Showtimes.Dtos;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Showtimes
{
    public class ShowtimeCustomerAppService : ApplicationService, IShowtimeCustomerAppService
    {
        private readonly IRepository<Showtime, Guid> _showtimeRepository;
        private readonly IRepository<Screen, Guid> _screenRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IMovieLookupClient _movieLookupClient;

        public ShowtimeCustomerAppService(
            IRepository<Showtime, Guid> showtimeRepository,
            IRepository<Screen, Guid> screenRepository,
            IDataFilter dataFilter,
            IMovieLookupClient movieLookupClient)
        {
            _showtimeRepository = showtimeRepository;
            _screenRepository = screenRepository;
            _dataFilter = dataFilter;
            _movieLookupClient = movieLookupClient;
        }

        public async Task<List<ShowtimeCustomerOutputDto>> GetListAsync(GetShowtimeCustomerListInputDto input)
        {
            input ??= new GetShowtimeCustomerListInputDto();

            // Public listing must surface showtimes across all tenants because the customer site
            // is unauthenticated and will not send a tenant header.
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var query = await _showtimeRepository.GetQueryableAsync();

                if (input.MovieId.HasValue)
                {
                    query = query.Where(x => x.MovieId == input.MovieId.Value);
                }

                if (input.CinemaId.HasValue)
                {
                    query = query.Where(x => x.CinemaId == input.CinemaId.Value);
                }

                if (input.Date.HasValue)
                {
                    var targetDate = input.Date.Value.Date;
                    query = query.Where(x => x.ShowDate.Date == targetDate);
                }

                var showtimes = await query.OrderBy(x => x.ShowDate).ThenBy(x => x.StartTime).ToListAsync();

                var screenNameById = await GetScreenNameMapAsync(showtimes);
                var movieById = new Dictionary<Guid, MovieLookupDto>();
                foreach (var group in showtimes.GroupBy(x => x.TenantId))
                {
                    var mapped = await _movieLookupClient.GetByIdsAsync(
                        group.Select(x => x.MovieId),
                        group.Key);
                    foreach (var pair in mapped)
                    {
                        movieById[pair.Key] = pair.Value;
                    }
                }

                return showtimes.Select(x => MapToDto(x, screenNameById, movieById)).ToList();
            }
        }

        public async Task<ShowtimeCustomerOutputDto> GetAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var query = await _showtimeRepository.GetQueryableAsync();
                var showtime = await query.FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new BusinessException("AdministrationService:ShowtimeNotFound").WithData("ShowtimeId", id);

                var screenNameById = await GetScreenNameMapAsync(new[] { showtime });
                var movie = await _movieLookupClient.GetByIdAsync(showtime.MovieId, showtime.TenantId);
                var movieById = movie != null
                    ? new Dictionary<Guid, MovieLookupDto> { [showtime.MovieId] = movie }
                    : (IReadOnlyDictionary<Guid, MovieLookupDto>)new Dictionary<Guid, MovieLookupDto>();

                return MapToDto(showtime, screenNameById, movieById);
            }
        }

        private async Task<Dictionary<Guid, string>> GetScreenNameMapAsync(IReadOnlyCollection<Showtime> showtimes)
        {
            if (showtimes.Count == 0)
            {
                return new Dictionary<Guid, string>();
            }

            var screenIds = showtimes.Select(x => x.ScreenId).Distinct().ToList();
            var screenQueryable = await _screenRepository.GetQueryableAsync();
            var screens = await screenQueryable.Where(x => screenIds.Contains(x.Id)).ToListAsync();

            return screens.ToDictionary(
                x => x.Id,
                x => x.ScreenNumber > 0 ? $"Screen {x.ScreenNumber}" : "Screen");
        }

        private static ShowtimeCustomerOutputDto MapToDto(
            Showtime entity,
            IReadOnlyDictionary<Guid, string> screenNameById,
            IReadOnlyDictionary<Guid, MovieLookupDto> movieById)
        {
            // Compose ISO 8601 strings so the FE can `dayjs()` them directly.
            var startInstant = entity.ShowDate.Date.Add(entity.StartTime);
            var endInstant = entity.ShowDate.Date.Add(entity.EndTime);

            screenNameById.TryGetValue(entity.ScreenId, out var screenName);
            movieById.TryGetValue(entity.MovieId, out var movie);

            return new ShowtimeCustomerOutputDto
            {
                Id = entity.Id,
                CinemaId = entity.CinemaId,
                MovieId = entity.MovieId,
                ScreenId = entity.ScreenId,
                StartTime = startInstant.ToString("o", CultureInfo.InvariantCulture),
                EndTime = endInstant.ToString("o", CultureInfo.InvariantCulture),
                TicketPrice = entity.BasePrice,
                Status = entity.Status,
                // The admin Showtime entity stores a JSON descriptor in MovieFormat; the customer
                // FE expects just the format string (e.g. "2D"), so unwrap it when possible.
                MovieFormat = ExtractMovieFormatString(entity.MovieFormat),
                ScreenName = screenName,
                DurationMins = entity.Duration > 0 ? entity.Duration : movie?.DurationMins,
                Movie = movie
            };
        }

        private static string? ExtractMovieFormatString(string? movieFormatJson)
        {
            if (string.IsNullOrWhiteSpace(movieFormatJson))
            {
                return null;
            }

            try
            {
                using var document = JsonDocument.Parse(movieFormatJson);
                if (document.RootElement.ValueKind == JsonValueKind.Object &&
                    document.RootElement.TryGetProperty("movie_format", out var movieFormat) &&
                    movieFormat.ValueKind == JsonValueKind.String)
                {
                    return movieFormat.GetString();
                }
            }
            catch (JsonException)
            {
                // Fall through and return the raw string for legacy data.
            }

            return movieFormatJson;
        }
    }
}
