using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LTC.AdministrationService.Showtimes.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace LTC.AdministrationService.Movies
{
    public class MovieLookupClient : IMovieLookupClient, ITransientDependency
    {
        public const string HttpClientName = "MovieService";
        private const string MoviePathTemplate = "movie/{0}";
        private const int MaxParallelism = 8;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MovieLookupClient> _logger;

        public MovieLookupClient(
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            ILogger<MovieLookupClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<MovieLookupDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var cacheKey = BuildCacheKey(id);
            if (_memoryCache.TryGetValue<MovieLookupDto>(cacheKey, out var cached))
            {
                return cached;
            }

            var movie = await FetchMovieAsync(id, cancellationToken);
            if (movie != null)
            {
                _memoryCache.Set(cacheKey, movie, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });
            }

            return movie;
        }

        public async Task<IReadOnlyDictionary<Guid, MovieLookupDto>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            var distinct = ids
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var result = new ConcurrentDictionary<Guid, MovieLookupDto>();
            if (distinct.Count == 0)
            {
                return result;
            }

            var pendingIds = new List<Guid>(distinct.Count);
            foreach (var id in distinct)
            {
                if (_memoryCache.TryGetValue<MovieLookupDto>(BuildCacheKey(id), out var cached) && cached != null)
                {
                    result[id] = cached;
                }
                else
                {
                    pendingIds.Add(id);
                }
            }

            if (pendingIds.Count == 0)
            {
                return result;
            }

            using var gate = new SemaphoreSlim(MaxParallelism);
            var tasks = pendingIds.Select(async id =>
            {
                await gate.WaitAsync(cancellationToken);
                try
                {
                    var movie = await FetchMovieAsync(id, cancellationToken);
                    if (movie != null)
                    {
                        _memoryCache.Set(BuildCacheKey(id), movie, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = CacheDuration
                        });
                        result[id] = movie;
                    }
                }
                finally
                {
                    gate.Release();
                }
            });

            await Task.WhenAll(tasks);
            return result;
        }

        private async Task<MovieLookupDto?> FetchMovieAsync(Guid id, CancellationToken cancellationToken)
        {
            HttpClient client;
            try
            {
                client = _httpClientFactory.CreateClient(HttpClientName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Movie lookup client could not be created; returning null for {MovieId}.", id);
                return null;
            }

            try
            {
                var requestUri = string.Format(MoviePathTemplate, id);
                using var response = await client.GetAsync(requestUri, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Movie lookup for {MovieId} failed with status {StatusCode}.",
                        id,
                        (int)response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<MovieLookupDto>(JsonOptions, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Movie lookup for {MovieId} failed; returning null.", id);
                return null;
            }
        }

        private static string BuildCacheKey(Guid id) => $"movie-lookup:{id}";
    }
}
