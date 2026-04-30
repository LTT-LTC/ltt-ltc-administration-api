using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;

namespace LTC.AdministrationService.Admin.Cinemas
{
    public interface IAdminCinemaAmenityAppService : IApplicationService
    {
        Task<PagedResultDto<CinemaAmenityOutputDto>> GetCinemaAmenityListAsync(Guid cinemaId, GetCinemaAmenityListInputDto input);
        Task<CinemaAmenityOutputDto> GetCinemaAmenityAsync(Guid cinemaId, Guid id);
        Task<CinemaAmenityOutputDto> CreateCinemaAmenityAsync(Guid cinemaId, CreateCinemaAmenityInputDto input);
        Task<CinemaAmenityOutputDto> UpdateCinemaAmenityAsync(Guid cinemaId, Guid id, UpdateCinemaAmenityInputDto input);
        Task DeleteCinemaAmenityAsync(Guid cinemaId, Guid id);
    }

    public class GetCinemaAmenityListInputDto : PagedAndSortedResultRequestDto
    {
        public string? Keyword { get; set; }
    }
}
