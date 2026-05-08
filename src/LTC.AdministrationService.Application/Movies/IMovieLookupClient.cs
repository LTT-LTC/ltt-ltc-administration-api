using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LTC.AdministrationService.Showtimes.Dtos;

namespace LTC.AdministrationService.Movies
{
    /// <summary>
    /// Resolves movie metadata from the movie microservice for read-only enrichment
    /// of showtime DTOs. Implementations should be resilient: returning <c>null</c>
    /// when the upstream movie service is unavailable rather than throwing.
    /// </summary>
    public interface IMovieLookupClient
    {
        Task<MovieLookupDto?> GetByIdAsync(
            Guid id,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<Guid, MovieLookupDto>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default);
    }
}
