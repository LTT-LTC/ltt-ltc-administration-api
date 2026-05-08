using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Cinemas;
using LTC.AdministrationService.Customer.Cinemas.Dtos.Input;
using LTC.AdministrationService.Customer.Cinemas.Dtos.Output;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Cinemas
{
    public class CinemaCustomerAppService : ApplicationService, ICinemaCustomerAppService
    {
        private readonly IRepository<Entities.Cinema, Guid> _cinemaRepository;
        private readonly IDataFilter _dataFilter;

        public CinemaCustomerAppService(
            IRepository<Entities.Cinema, Guid> cinemaRepository,
            IDataFilter dataFilter)
        {
            _cinemaRepository = cinemaRepository;
            _dataFilter = dataFilter;
        }

        public async Task<List<CinemaCustomerOutputDto>> GetListAsync(GetCinemaCustomerListInputDto input)
        {
            input ??= new GetCinemaCustomerListInputDto();

            var keyword = (input.Keyword ?? string.Empty).Trim();
            var city = (input.City ?? string.Empty).Trim();
            var status = (input.Status ?? string.Empty).Trim();
            var page = input.Page > 0 ? input.Page : 1;
            var pageSize = input.PageSize > 0 ? input.PageSize : 100;

            // Public listing must surface cinemas across all tenants because the customer site
            // is unauthenticated and will not send a tenant header.
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var queryable = await _cinemaRepository.GetQueryableAsync();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    queryable = queryable.Where(x =>
                        x.Name.Contains(keyword) ||
                        (x.City != null && x.City.Contains(keyword)) ||
                        (x.Address != null && x.Address.Contains(keyword)));
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    queryable = queryable.Where(x => x.City != null && x.City.Contains(city));
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    queryable = queryable.Where(x => x.Status == status);
                }

                var items = await queryable
                    .OrderBy(x => x.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return items
                    .Select(x => ObjectMapper.Map<Entities.Cinema, CinemaCustomerOutputDto>(x))
                    .ToList();
            }
        }

        public async Task<CinemaCustomerOutputDto> GetAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var queryable = await _cinemaRepository.GetQueryableAsync();
                var cinema = await queryable.FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new BusinessException("AdministrationService:CinemaNotFound").WithData("CinemaId", id);

                return ObjectMapper.Map<Entities.Cinema, CinemaCustomerOutputDto>(cinema);
            }
        }
    }
}
