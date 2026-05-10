using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Showtimes.Dtos;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Showtimes;

public interface IShowtimeSeatLayoutMergeAppService : IApplicationService
{
    Task MergePaidSeatsAsync(Guid showtimeId, MergePaidShowtimeSeatsInputDto input);
}
