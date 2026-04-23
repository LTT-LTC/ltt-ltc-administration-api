using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager CinemaAmenity operations: read-only access.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/cinema/{cinemaId}/amenity")]
    public class CinemaAmenityManagerController : ManagerControllerBase
    {
        private readonly IAdminCinemaAmenityAppService _amenityAppService;

        public CinemaAmenityManagerController(IAdminCinemaAmenityAppService amenityAppService)
        {
            _amenityAppService = amenityAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CinemaAmenityOutputDto>> GetListAsync(Guid cinemaId, GetCinemaAmenityListInputDto input)
        {
            return _amenityAppService.GetListAsync(cinemaId, input);
        }

        [HttpGet("{id}")]
        public virtual Task<CinemaAmenityOutputDto> GetAsync(Guid cinemaId, Guid id)
        {
            return _amenityAppService.GetAsync(cinemaId, id);
        }
    }
}
