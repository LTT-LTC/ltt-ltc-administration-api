using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;
using LTC.Shared.Hosting.Microservices.Timing;
using Volo.Abp;
using Volo.Abp.Identity;

namespace LTC.AdministrationService.Cinemas
{
    public class CinemaAppService : ApplicationService, IAdminCinemaAppService
    {
        private readonly IRepository<Entities.Cinema, Guid> _cinemaRepository;
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IGmt7Clock _gmt7Clock;

        public CinemaAppService(
            IRepository<Entities.Cinema, Guid> cinemaRepository,
            IRepository<IdentityUser, Guid> identityUserRepository,
            IdentityUserManager identityUserManager,
            IGmt7Clock gmt7Clock)
        {
            _cinemaRepository = cinemaRepository;
            _identityUserRepository = identityUserRepository;
            _identityUserManager = identityUserManager;
            _gmt7Clock = gmt7Clock;
        }

        private async Task ValidateManagerUserAsync(Guid? managerUserId, Guid? updatingCinemaId = null)
        {
            if (!managerUserId.HasValue)
            {
                return;
            }

            var user = await _identityUserRepository.FindAsync(managerUserId.Value)
                ?? throw new UserFriendlyException("Manager user not found.");

            var roles = await _identityUserManager.GetRolesAsync(user);
            if (!roles.Any(r => string.Equals(r, "Manager", StringComparison.OrdinalIgnoreCase)))
            {
                throw new UserFriendlyException("Assigned manager user must have Manager role.");
            }

            var queryable = await _cinemaRepository.GetQueryableAsync();
            var conflict = await queryable.AnyAsync(x =>
                x.ManagerUserId == managerUserId.Value
                && (!updatingCinemaId.HasValue || x.Id != updatingCinemaId.Value));
            if (conflict)
            {
                throw new UserFriendlyException("Manager is already assigned to another cinema.");
            }
        }

        public async Task<PagedResultDto<CinemasOutputDto>> GetListAsync(GetCinemasListInputDto input)
        {
            var queryable = await _cinemaRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryable = queryable.Where(x => x.Name.Contains(input.Keyword) || (x.City != null && x.City.Contains(input.Keyword)));
            }

            if (!string.IsNullOrWhiteSpace(input.Status))
            {
                queryable = queryable.Where(x => x.Status == input.Status);
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderBy(x => x.Name)
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToListAsync();

            return new PagedResultDto<CinemasOutputDto>(
                totalCount,
                items.Select(x => ObjectMapper.Map<Entities.Cinema, CinemasOutputDto>(x)).ToList()
            );
        }

        public async Task<CinemasOutputDto> GetAsync(Guid id)
        {
            var cinema = await _cinemaRepository.GetAsync(id);
            return ObjectMapper.Map<Entities.Cinema, CinemasOutputDto>(cinema);
        }

        public async Task<CinemasOutputDto> CreateAsync(CreateCinemasInputDto input)
        {
            await ValidateManagerUserAsync(input.ManagerUserId);
            var cinema = ObjectMapper.Map<CreateCinemasInputDto, Entities.Cinema>(input);
            cinema.CreatedAt = _gmt7Clock.Gmt7Now;

            cinema = await _cinemaRepository.InsertAsync(cinema, autoSave: true);
            return ObjectMapper.Map<Entities.Cinema, CinemasOutputDto>(cinema);
        }

        public async Task<CinemasOutputDto> UpdateAsync(Guid id, UpdateCinemasInputDto input)
        {
            await ValidateManagerUserAsync(input.ManagerUserId, id);
            var cinema = await _cinemaRepository.GetAsync(id);
            ObjectMapper.Map(input, cinema);
            cinema.UpdatedAt = _gmt7Clock.Gmt7Now;

            cinema = await _cinemaRepository.UpdateAsync(cinema, autoSave: true);
            return ObjectMapper.Map<Entities.Cinema, CinemasOutputDto>(cinema);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _cinemaRepository.DeleteAsync(id, autoSave: true);
        }
    }
}