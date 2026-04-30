using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.NewsAndOffers.Dtos;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;
using LTC.AdministrationService.NewsAndOffers;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager NewsAndOffers operations: read, create, and update (no delete).
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/news-and-offers")]
    public class NewsAndOffersManagerController : ManagerControllerBase
    {
        private readonly INewsAndOffersAppService _newsAndOffersService;

        public NewsAndOffersManagerController(INewsAndOffersAppService newsAndOffersService)
        {
            _newsAndOffersService = newsAndOffersService;
        }

        [HttpGet]
        public virtual async Task<PagedResultDto<NewsAndOffersOutputDto>> GetNewsAndOffersListAsync([FromQuery] GetNewsAndOffersListinputDto input)
        {
            return await _newsAndOffersService.GetNewsAndOffersListAsync(input);
        }

        [HttpGet("{id}")]
        public async Task<NewsAndOffersOutputDto> GetNewsAndOffersAsync(Guid id)
        {
            return await _newsAndOffersService.GetNewsAndOffersAsync(id);
        }

        [HttpPost]
        public async Task<NewsAndOffersOutputDto> CreateNewsAndOffersAsync([FromForm] CreateNewsAndOffersDto input)
        {
            return await _newsAndOffersService.CreateNewsAndOffersAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<NewsAndOffersOutputDto> UpdateNewsAndOffersAsync(Guid id, [FromForm] UpdateNewsAndOffersDto input)
        {
            return await _newsAndOffersService.UpdateNewsAndOffersAsync(id, input);
        }

        [HttpDelete("{id}")]
        public async Task DeleteNewsAndOffersAsync(Guid id)
        {
            await _newsAndOffersService.DeleteNewsAndOffersAsync(id);
        }
    }
}
