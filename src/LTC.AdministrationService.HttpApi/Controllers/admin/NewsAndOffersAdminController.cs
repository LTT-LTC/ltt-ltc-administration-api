using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LTC.AdministrationService.NewsAndOffers.Dtos;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;
using LTC.AdministrationService.NewsAndOffers;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only NewsAndOffers operations: all CRUD including delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/news-and-offers")]
    public class NewsAndOffersAdminController : AdminControllerBase
    {
        private readonly INewsAndOffersAppService _newsAndOffersService;

        public NewsAndOffersAdminController(INewsAndOffersAppService newsAndOffersService)
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
        public async Task<NewsAndOffersOutputDto> DeleteNewsAndOffersAsync(Guid id)
        {
            return await _newsAndOffersService.DeleteNewsAndOffersAsync(id);
        }
    }
}
