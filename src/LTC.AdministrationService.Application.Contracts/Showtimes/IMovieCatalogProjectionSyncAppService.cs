using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using LTC.AdministrationService.Showtimes.Dtos;

namespace LTC.AdministrationService.Showtimes
{
    /// <summary>
    /// Synchronizes local movie/distribution read models from the Movie service.
    /// This keeps Showtime validation local and resilient.
    /// </summary>
    public interface IMovieCatalogProjectionSyncAppService : IApplicationService
    {
        Task UpsertMovieAsync(SyncMovieProjectionDto input);
        Task DeleteMovieAsync(Guid movieId);
        Task UpsertDistributionAsync(SyncMovieDistributionProjectionDto input);
        Task DeleteDistributionAsync(Guid distributionId);
    }
}
