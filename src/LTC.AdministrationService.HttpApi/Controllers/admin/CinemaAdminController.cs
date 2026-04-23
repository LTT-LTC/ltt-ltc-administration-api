using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only Cinema operations: create, update, delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/cinema")]
    public class CinemaAdminController : AdminControllerBase
    {
        private readonly IAdminCinemaAppService _cinemaAppService;

        public CinemaAdminController(IAdminCinemaAppService cinemaAppService)
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
