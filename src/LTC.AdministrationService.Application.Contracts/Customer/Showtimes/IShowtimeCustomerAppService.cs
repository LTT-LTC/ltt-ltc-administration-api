using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Input;
using LTC.AdministrationService.Customer.Showtimes.Dtos.Output;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Customer.Showtimes
{
    public interface IShowtimeCustomerAppService : IApplicationService
    {
        Task<List<ShowtimeCustomerOutputDto>> GetListAsync(GetShowtimeCustomerListInputDto input);
        Task<ShowtimeCustomerOutputDto> GetAsync(Guid id);
    }
}
