using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LTC.AdministrationService.NewsAndOffers.Dtos;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.NewsAndOffers
{
    public interface INewsAndOffersAppService
    {
        Task<PagedResultDto<NewsAndOffersOutputDto>> GetNewsAndOffersListAsync(GetNewsAndOffersListinputDto input);
        Task<NewsAndOffersOutputDto> GetNewsAndOffersAsync(Guid id);
        Task<NewsAndOffersOutputDto> CreateNewsAndOffersAsync(CreateNewsAndOffersDto input);
        Task<NewsAndOffersOutputDto> UpdateNewsAndOffersAsync(Guid id, UpdateNewsAndOffersDto input);
        Task<NewsAndOffersOutputDto> DeleteNewsAndOffersAsync(Guid id);
    }
}
