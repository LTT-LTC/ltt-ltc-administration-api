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
        public async Task<NewsAndOffersOutputDto> CreateAsync([FromForm] CreateNewsAndOffersDto input)
        {
            return await _newsAndOffersService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<NewsAndOffersOutputDto> UpdateAsync(Guid id, [FromForm] UpdateNewsAndOffersDto input)
        {
            return await _newsAndOffersService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _newsAndOffersService.DeleteAsync(id);
        }
    }
}
