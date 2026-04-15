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
        Task<PagedResultDto<NewsAndOffersOutputDto>> GetListAsync(GetNewsAndOffersListinputDto input);
        Task<NewsAndOffersOutputDto> GetAsync(Guid id);
        Task<NewsAndOffersOutputDto> CreateAsync(CreateNewsAndOffersDto input);
        Task<NewsAndOffersOutputDto> UpdateAsync(Guid id, UpdateNewsAndOffersDto input);
        Task<NewsAndOffersOutputDto> DeleteAsync(Guid id);
    }
}
