using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Cinemas;
using LTC.AdministrationService.Customer.Cinemas.Dtos.Input;
using LTC.AdministrationService.Customer.Cinemas.Dtos.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers.Customer;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/customer/cinema")]
[AllowAnonymous]
public class CinemaCustomerController : CustomerControllerBase
{
    private readonly ICinemaCustomerAppService _cinemaCustomerAppService;

    public CinemaCustomerController(ICinemaCustomerAppService cinemaCustomerAppService)
    {
        _cinemaCustomerAppService = cinemaCustomerAppService;
    }

    [HttpGet]
    public virtual Task<List<CinemaCustomerOutputDto>> GetCinemaListAsync([FromQuery] GetCinemaCustomerListInputDto input)
        => _cinemaCustomerAppService.GetListAsync(input ?? new GetCinemaCustomerListInputDto());

    [HttpGet("{id}")]
    public virtual Task<CinemaCustomerOutputDto> GetCinemaAsync(Guid id)
        => _cinemaCustomerAppService.GetAsync(id);
}
