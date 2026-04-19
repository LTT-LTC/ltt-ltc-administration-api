using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using LTC.AdministrationService.NewsAndOffers.Dtos;
using LTC.AdministrationService.Controllers;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;
using LTC.AdministrationService.NewsAndOffers;


namespace LTC.AdministrationService.Controllers
{
    [RemoteService]
    [Area("administration")]
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/news-and-offers")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class NewsAndOffersController : AbpController, IRemoteService
    {
        private readonly INewsAndOffersAppService _newsAndOffersService;

        public NewsAndOffersController(INewsAndOffersAppService newsAndOfferAppService)
        {
            _newsAndOffersService = newsAndOfferAppService;
        }

        [HttpGet]
        public virtual async Task<PagedResultDto<NewsAndOffersOutputDto>> GetListAsync([FromQuery] GetNewsAndOffersListinputDto input)
        {
            return await _newsAndOffersService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public async Task<NewsAndOffersOutputDto> GetAsync(Guid id)
        {
            return await _newsAndOffersService.GetAsync(id);
        }

        [HttpPost]
        public async Task<NewsAndOffersOutputDto> CreateAsync(CreateNewsAndOffersDto input)
        {
            return await _newsAndOffersService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<NewsAndOffersOutputDto> UpdateAsync(Guid id, UpdateNewsAndOffersDto input)
        {
            return await _newsAndOffersService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public async Task<NewsAndOffersOutputDto> DeleteAsync(Guid id)
        {
            return await _newsAndOffersService.DeleteAsync(id);
        }
    }
}
