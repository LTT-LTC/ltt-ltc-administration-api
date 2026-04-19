using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LTC.AdministrationService.Controllers;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;

namespace LTC.AdministrationService.Controllers
{
    [RemoteService]
    [Area("administration")]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/cinema")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class CinemaController : AdministrationServiceController
    {
        private readonly IAdminCinemaAppService _cinemaAppService;

        public CinemaController(IAdminCinemaAppService cinemaAppService)
        {
            _cinemaAppService = cinemaAppService;
        }

        [HttpGet]
        public virtual async Task<PagedResultDto<CinemasOutputDto>> GetListAsync([FromQuery] GetCinemasListInputDto input)
        {
            return await _cinemaAppService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public virtual async Task<CinemasOutputDto> GetAsync(Guid id)
        {
            return await _cinemaAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual async Task<CinemasOutputDto> CreateAsync(CreateCinemasInputDto input)
        {
            return await _cinemaAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public virtual async Task<CinemasOutputDto> UpdateAsync(Guid id, UpdateCinemasInputDto input)
        {
            return await _cinemaAppService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _cinemaAppService.DeleteAsync(id);
        }

    }
}
