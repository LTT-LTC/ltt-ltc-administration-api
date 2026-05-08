using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Input;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers.Customer
{
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/customer/showtimes")]
    [AllowAnonymous]
    public class ShowtimeCustomerController : CustomerControllerBase
    {
        private readonly IShowtimeCustomerAppService _showtimeCustomerAppService;

        public ShowtimeCustomerController(IShowtimeCustomerAppService showtimeCustomerAppService)
        {
            _showtimeCustomerAppService = showtimeCustomerAppService;
        }

        [HttpGet]
        public virtual Task<List<ShowtimeCustomerOutputDto>> GetShowtimeListAsync([FromQuery] GetShowtimeCustomerListInputDto input)
            => _showtimeCustomerAppService.GetListAsync(input ?? new GetShowtimeCustomerListInputDto());

        [HttpGet("{id}")]
        public virtual Task<ShowtimeCustomerOutputDto> GetShowtimeAsync(Guid id)
            => _showtimeCustomerAppService.GetAsync(id);
    }
}
