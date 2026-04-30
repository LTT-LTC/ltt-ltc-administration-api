using System;
using System.Threading.Tasks;
using LTC.AdministrationService.NewsAndOffers;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Controllers.Customer;

[Route(AdministrationServiceSettingNames.DefaultRoute + "/customer/news-and-offers")]
[AllowAnonymous]
public class NewsAndOffersCustomerController : CustomerControllerBase
{
    private readonly INewsAndOffersAppService _newsAndOffersAppService;

    public NewsAndOffersCustomerController(INewsAndOffersAppService newsAndOffersAppService)
    {
        _newsAndOffersAppService = newsAndOffersAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<NewsAndOffersOutputDto>> GetNewsAndOffersListAsync([FromQuery] GetNewsAndOffersListinputDto input)
        => _newsAndOffersAppService.GetNewsAndOffersListAsync(input);

    [HttpGet("{id}")]
    public virtual Task<NewsAndOffersOutputDto> GetNewsAndOffersAsync(Guid id)
        => _newsAndOffersAppService.GetNewsAndOffersAsync(id);
}
