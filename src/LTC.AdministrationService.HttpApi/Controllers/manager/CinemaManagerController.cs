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
    /// Manager Cinema operations: read-only access to cinema list and details.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/cinema")]
    public class CinemaManagerController : ManagerControllerBase
    {
        private readonly IAdminCinemaAppService _cinemaAppService;

        public CinemaManagerController(IAdminCinemaAppService cinemaAppService)
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
    }
}
