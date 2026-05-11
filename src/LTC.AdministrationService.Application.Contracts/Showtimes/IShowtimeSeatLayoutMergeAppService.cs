using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Showtimes.Dtos;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Showtimes;

public interface IShowtimeSeatLayoutMergeAppService : IApplicationService
{
    Task MergePaidSeatsAsync(Guid showtimeId, MergePaidShowtimeSeatsInputDto input);

    /// <summary>
    /// Persists seat holds in the showtime database when user confirms seat selection.
    /// This provides durable locking beyond the Redis TTL period.
    /// </summary>
    Task HoldSeatsAsync(Guid showtimeId, HoldShowtimeSeatsInputDto input);
}
