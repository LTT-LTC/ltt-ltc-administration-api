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
using Volo.Abp.TenantManagement;

namespace LTC.AdministrationService.Movies
{
    public class MovieLookupClient : IMovieLookupClient, ITransientDependency
    {
        public const string HttpClientName = "MovieService";
        public const string GatewayHttpClientName = "MovieServiceGateway";
        private static readonly string[] MoviePathTemplates =
        {
            "movie/{0}",
            "/ltc/movie-service/movie/{0}",
            "/movie-service/movie/{0}",
            "/movie/{0}"
        };
        private const int MaxParallelism = 8;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MovieLookupClient> _logger;
        private readonly ITenantRepository _tenantRepository;

        public MovieLookupClient(
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            ILogger<MovieLookupClient> logger,
            ITenantRepository tenantRepository)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
            _tenantRepository = tenantRepository;
        }

        public async Task<MovieLookupDto?> GetByIdAsync(
            Guid id,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var cacheKey = BuildCacheKey(id, tenantId);
            if (_memoryCache.TryGetValue<MovieLookupDto>(cacheKey, out var cached))
            {
                return cached;
            }

            var movie = await FetchMovieAsync(id, tenantId, cancellationToken);
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
            Guid? tenantId = null,
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
                if (_memoryCache.TryGetValue<MovieLookupDto>(BuildCacheKey(id, tenantId), out var cached) && cached != null)
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
                    var movie = await FetchMovieAsync(id, tenantId, cancellationToken);
                    if (movie != null)
                    {
                        _memoryCache.Set(BuildCacheKey(id, tenantId), movie, new MemoryCacheEntryOptions
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

        private async Task<MovieLookupDto?> FetchMovieAsync(
            Guid id,
            Guid? tenantId,
            CancellationToken cancellationToken)
        {
            var clientNames = new[] { HttpClientName, GatewayHttpClientName };
            foreach (var clientName in clientNames)
            {
                HttpClient client;
                try
                {
                    client = _httpClientFactory.CreateClient(clientName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Movie lookup client {ClientName} could not be created for {MovieId}.", clientName, id);
                    continue;
                }

                try
                {
                    var tenantName = await ResolveTenantNameAsync(tenantId);
                    foreach (var template in MoviePathTemplates)
                    {
                        var requestUri = string.Format(template, id);
                        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                        if (!string.IsNullOrWhiteSpace(tenantName))
                        {
                            // movie-service validates X-Tenant against ITenantStore by tenant Name.
                            request.Headers.TryAddWithoutValidation("X-Tenant", tenantName);
                        }
                        if (tenantId.HasValue)
                        {
                            // Keep __tenant as id to support ABP tenant resolution when configured.
                            request.Headers.TryAddWithoutValidation("__tenant", tenantId.Value.ToString());
                        }

                        using var response = await client.SendAsync(request, cancellationToken);

                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            // Try the next URL shape/client before giving up.
                            continue;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogWarning(
                                "Movie lookup for {MovieId} via {ClientName}:{RequestUri} failed with status {StatusCode}.",
                                id,
                                clientName,
                                requestUri,
                                (int)response.StatusCode);
                            continue;
                        }

                        var payload = await response.Content.ReadFromJsonAsync<MovieLookupDto>(JsonOptions, cancellationToken);
                        if (payload != null)
                        {
                            return payload;
                        }
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Movie lookup for {MovieId} failed via client {ClientName}.", id, clientName);
                }
            }

            return null;
        }

        private async Task<string?> ResolveTenantNameAsync(Guid? tenantId)
        {
            if (!tenantId.HasValue)
            {
                return null;
            }

            var cacheKey = $"movie-lookup:tenant-name:{tenantId.Value}";
            if (_memoryCache.TryGetValue<string>(cacheKey, out var cached))
            {
                return cached;
            }

            try
            {
                var tenant = await _tenantRepository.FindAsync(tenantId.Value);
                var tenantName = tenant?.Name;
                if (!string.IsNullOrWhiteSpace(tenantName))
                {
                    _memoryCache.Set(cacheKey, tenantName, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheDuration
                    });
                    return tenantName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve tenant name for tenantId {TenantId}.", tenantId);
            }

            return null;
        }

        private static string BuildCacheKey(Guid id, Guid? tenantId) => $"movie-lookup:{tenantId?.ToString() ?? "host"}:{id}";
    }
}
