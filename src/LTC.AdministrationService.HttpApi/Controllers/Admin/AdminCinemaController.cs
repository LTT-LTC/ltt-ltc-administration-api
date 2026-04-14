using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.Controllers;
using LTC.AdministrationService.Admin.Cinema;
using LTC.AdministrationService.Admin.Cinema.Dtos.Input;
using LTC.AdministrationService.Admin.Cinema.Dtos.Output;

namespace LTC.AdministrationService.HttpApi.Controllers.Admin
{
    [RemoteService]
    [Area("administration")]
    [Route("ltc/administration-service/api/administration/admin/cinema")]
    [ApiController]
    public class AdminCinemaController : AdministrationServiceController
    {
        private readonly IAdminCinemaAppService _cinemaAppService;

        public AdminCinemaController(IAdminCinemaAppService cinemaAppService)
        {
            _cinemaAppService = cinemaAppService;
        }

        [HttpGet]
        public virtual async Task<PagedResultDto<CinemaOutputDto>> GetListAsync([FromQuery] GetCinemaListInputDto input)
        {
            return await _cinemaAppService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public virtual async Task<CinemaOutputDto> GetAsync(Guid id)
        {
            return await _cinemaAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual async Task<CinemaOutputDto> CreateAsync(CreateCinemaInputDto input)
        {
            return await _cinemaAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public virtual async Task<CinemaOutputDto> UpdateAsync(Guid id, UpdateCinemaInputDto input)
        {
            return await _cinemaAppService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _cinemaAppService.DeleteAsync(id);
        }

        // Admin assigns amenities to cinema
        [Route("{cinemaId}/amenity")]
        [HttpPost] public virtual async Task<IActionResult> AssignAmenityAsync(Guid cinemaId) => Ok();
        [Route("{cinemaId}/amenity/{amenityId}")]
        [HttpDelete] public virtual async Task<IActionResult> RemoveAmenityAsync(Guid cinemaId, Guid amenityId) => Ok();
    }
}
