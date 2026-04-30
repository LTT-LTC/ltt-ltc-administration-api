using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Showtimes;
using LTC.AdministrationService.Showtimes.Dtos;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager Showtime operations: read and write access per policy.
    /// Managers can GET, POST, and PUT showtimes.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/showtimes")]
    public class ShowtimeManagerController : ManagerControllerBase
    {
        private readonly IShowtimeAppService _showtimeAppService;

        public ShowtimeManagerController(IShowtimeAppService showtimeAppService)
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

        [HttpPost]
        public async Task<IActionResult> CreateShowtimeAsync([FromBody] CreateShowtimeDto input)
        {
            var result = await _showtimeAppService.CreateShowtimeAsync(input);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowtimeAsync(Guid id, [FromBody] CreateShowtimeDto input)
        {
            var result = await _showtimeAppService.UpdateShowtimeAsync(id, input);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowtimeAsync(Guid id)
        {
            await _showtimeAppService.DeleteShowtimeAsync(id);
            return Ok();
        }
    }
}
