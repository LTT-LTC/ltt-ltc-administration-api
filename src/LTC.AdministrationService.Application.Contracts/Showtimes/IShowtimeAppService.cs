using System;
using Volo.Abp.Application.Services;
using LTC.AdministrationService.Showtimes.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Showtimes
{
    public interface IShowtimeAppService : IApplicationService
    {
        Task<PagedResultDto<ShowtimeOutputDto>> GetListAsync(Guid movieId, Guid? cinemaId, int skipCount, int maxResultCount);
        Task<ShowtimeOutputDto> GetAsync(Guid id);
        Task<ShowtimeOutputDto> CreateAsync(CreateShowtimeDto input);
        Task<ShowtimeOutputDto> UpdateAsync(Guid id, CreateShowtimeDto input);
        Task DeleteAsync(Guid id);
    }
}