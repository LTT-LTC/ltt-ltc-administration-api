using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Cinemas.Dtos.Input;
using LTC.AdministrationService.Customer.Cinemas.Dtos.Output;
using Volo.Abp.Application.Services;

namespace LTC.AdministrationService.Customer.Cinemas
{
    public interface ICinemaCustomerAppService : IApplicationService
    {
        Task<List<CinemaCustomerOutputDto>> GetListAsync(GetCinemaCustomerListInputDto input);
        Task<CinemaCustomerOutputDto> GetAsync(Guid id);
    }
}
