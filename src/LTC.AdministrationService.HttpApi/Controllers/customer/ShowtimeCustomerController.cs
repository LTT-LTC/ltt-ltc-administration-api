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
        public async Task<PagedResultDto<ShowtimeOutputDto>> GetListByMovieAsync(Guid movieId, Guid? cinemaId = null, int skipCount = 0, int maxResultCount = 10)
        {
            return await _showtimeAppService.GetListAsync(movieId, cinemaId, skipCount, maxResultCount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var result = await _showtimeAppService.GetAsync(id);
            return Ok(result);
        }
    }
}
