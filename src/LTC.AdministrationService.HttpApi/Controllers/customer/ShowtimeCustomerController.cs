using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Showtimes;
using LTC.AdministrationService.Showtimes.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Customer
{
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/customer/showtimes")]
    public class ShowtimeCustomerController : CustomerControllerBase
    {
        private readonly IShowtimeAppService _showtimeAppService;

        public ShowtimeCustomerController(IShowtimeAppService showtimeAppService)
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
