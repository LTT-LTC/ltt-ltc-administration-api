using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Showtimes;
using LTC.AdministrationService.Showtimes.Dtos;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin Showtime operations are read-only.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/showtimes")]
    public class ShowtimeAdminController : AdminControllerBase
    {
        private readonly IShowtimeAppService _showtimeAppService;

        public ShowtimeAdminController(IShowtimeAppService showtimeAppService)
        {
            _showtimeAppService = showtimeAppService;
        }

        [HttpGet("movie/{movieId}")]
        public async Task<PagedResultDto<ShowtimeOutputDto>> GetShowtimeListByMovieAsync(Guid movieId, Guid? cinemaId = null, int skipCount = 0, int maxResultCount = 10)
        {
            return await _showtimeAppService.GetShowtimeListAsync(movieId, cinemaId, skipCount, maxResultCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowtimeAsync(Guid id)
        {
            var result = await _showtimeAppService.GetShowtimeAsync(id);
            return Ok(result);
        }
    }
}
