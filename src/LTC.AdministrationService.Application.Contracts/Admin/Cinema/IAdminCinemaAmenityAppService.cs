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
        Task<PagedResultDto<CinemaAmenityOutputDto>> GetListAsync(Guid cinemaId, GetCinemaAmenityListInputDto input);
        Task<CinemaAmenityOutputDto> GetAsync(Guid cinemaId, Guid id);
        Task<CinemaAmenityOutputDto> CreateAsync(Guid cinemaId, CreateCinemaAmenityInputDto input);
        Task<CinemaAmenityOutputDto> UpdateAsync(Guid cinemaId, Guid id, UpdateCinemaAmenityInputDto input);
        Task DeleteAsync(Guid cinemaId, Guid id);
    }

    public class GetCinemaAmenityListInputDto : PagedAndSortedResultRequestDto
    {
        public string? Keyword { get; set; }
    }
}
