using System;
using Volo.Abp.Application.Services;
using LTC.AdministrationService.Showtimes.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Showtimes
{
    public interface IShowtimeAppService : IApplicationService
    {
        Task<PagedResultDto<ShowtimeOutputDto>> GetShowtimeListAsync(Guid movieId, Guid? cinemaId, int skipCount, int maxResultCount);
        Task<ShowtimeOutputDto> GetShowtimeAsync(Guid id);
        Task<ShowtimeOutputDto> CreateShowtimeAsync(CreateShowtimeDto input);
        Task<ShowtimeOutputDto> UpdateShowtimeAsync(Guid id, CreateShowtimeDto input);
        Task DeleteShowtimeAsync(Guid id);
    }
}