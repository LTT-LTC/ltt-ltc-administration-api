using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only CinemaAmenity operations: create, update, delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/cinema/{cinemaId}/amenity")]
    public class CinemaAmenityAdminController : AdminControllerBase, IAdminCinemaAmenityAppService
    {
        private readonly IAdminCinemaAmenityAppService _amenityAppService;

        public CinemaAmenityAdminController(IAdminCinemaAmenityAppService amenityAppService)
        {
            _amenityAppService = amenityAppService;
        }

        [HttpPost]
        public virtual Task<CinemaAmenityOutputDto> CreateAsync(Guid cinemaId, CreateCinemaAmenityInputDto input)
        {
            return _amenityAppService.CreateAsync(cinemaId, input);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CinemaAmenityOutputDto>> GetListAsync(Guid cinemaId, [FromQuery] GetCinemaAmenityListInputDto input)
        {
            return _amenityAppService.GetListAsync(cinemaId, input);
        }

        [HttpGet("{id}")]
        public virtual Task<CinemaAmenityOutputDto> GetAsync(Guid cinemaId, Guid id)
        {
            return _amenityAppService.GetAsync(cinemaId, id);
        }

        [HttpPut("{id}")]
        public virtual Task<CinemaAmenityOutputDto> UpdateAsync(Guid cinemaId, Guid id, UpdateCinemaAmenityInputDto input)
        {
            return _amenityAppService.UpdateAsync(cinemaId, id, input);
        }

        [HttpDelete("{id}")]
        public virtual Task DeleteAsync(Guid cinemaId, Guid id)
        {
            return _amenityAppService.DeleteAsync(cinemaId, id);
        }
    }
}
