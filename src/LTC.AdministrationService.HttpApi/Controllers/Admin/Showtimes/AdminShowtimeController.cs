using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.Showtimes;
using LTC.AdministrationService.Showtimes.Dtos;

namespace LTC.AdministrationService.Controllers.Admin.Showtimes
{
    [RemoteService]
    [Area("administration")]
    [Route("ltc/administration-service/api/administration/admin/showtimes")]
    public class AdminShowtimeController : AdministrationServiceController
    {
        private readonly IShowtimeAppService _showtimeAppService;

        public AdminShowtimeController(IShowtimeAppService showtimeAppService)
        {
            _showtimeAppService = showtimeAppService;
        }

        [HttpGet]
        public async Task<PagedResultDto<ShowtimeOutputDto>> GetListAsync(Guid cinemaId, int skipCount = 0, int maxResultCount = 10)
        {
            return await _showtimeAppService.GetListAsync(cinemaId, skipCount, maxResultCount);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateShowtimeDto input)
        {
            var result = await _showtimeAppService.CreateAsync(input);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _showtimeAppService.DeleteAsync(id);
            return Ok();
        }
    }
}