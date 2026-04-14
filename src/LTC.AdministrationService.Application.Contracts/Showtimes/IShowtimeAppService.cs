using System;
using Volo.Abp.Application.Services;
using LTC.AdministrationService.Showtimes.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Showtimes
{
    public interface IShowtimeAppService : IApplicationService
    {
        Task<PagedResultDto<ShowtimeOutputDto>> GetListAsync(Guid cinemaId, int skipCount, int maxResultCount);
        Task<ShowtimeOutputDto> CreateAsync(CreateShowtimeDto input);
        Task DeleteAsync(Guid id);
    }
}