using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;

namespace LTC.AdministrationService.Controllers
{
    [RemoteService(Name = "AdministrationService")]
    [Area("administration")]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/cinema/{cinemaId}/amenity")]
    [Authorize(Roles = "Admin,Manager")]
    public class CinemaAmenityController : AbpController, IAdminCinemaAmenityAppService
    {
        private readonly IAdminCinemaAmenityAppService _amenityAppService;

        public CinemaAmenityController(IAdminCinemaAmenityAppService amenityAppService)
        {
            _amenityAppService = amenityAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CinemaAmenityOutputDto>> GetListAsync(Guid cinemaId, GetCinemaAmenityListInputDto input)
        {
            return _amenityAppService.GetListAsync(cinemaId, input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<CinemaAmenityOutputDto> GetAsync(Guid cinemaId, Guid id)
        {
            return _amenityAppService.GetAsync(cinemaId, id);
        }

        [HttpPost]
        public virtual Task<CinemaAmenityOutputDto> CreateAsync(Guid cinemaId, CreateCinemaAmenityInputDto input)
        {
            return _amenityAppService.CreateAsync(cinemaId, input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<CinemaAmenityOutputDto> UpdateAsync(Guid cinemaId, Guid id, UpdateCinemaAmenityInputDto input)
        {
            return _amenityAppService.UpdateAsync(cinemaId, id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid cinemaId, Guid id)
        {
            return _amenityAppService.DeleteAsync(cinemaId, id);
        }
    }
}