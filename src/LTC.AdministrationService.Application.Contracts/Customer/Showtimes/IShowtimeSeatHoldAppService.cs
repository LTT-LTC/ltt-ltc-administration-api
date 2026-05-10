using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Input;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Output;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Customer.Showtimes;

public interface IShowtimeSeatHoldAppService : IApplicationService
{
    Task<HoldSeatsOutputDto> HoldAsync(HoldSeatsInputDto input);

    Task ReleaseAsync(Guid showtimeId, string sessionKey);
}
