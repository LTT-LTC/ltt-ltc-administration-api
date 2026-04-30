using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LTC.AdministrationService.Admin.SeatMaps;
using LTC.AdministrationService.Admin.SeatMaps.Dtos.Input;
using LTC.AdministrationService.Admin.SeatMaps.Dtos.Output;
using LTC.AdministrationService.Entities;
using LTC.Shared.Hosting.Microservices.Timing;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace LTC.AdministrationService.SeatMaps
{
    public class SeatMapAppService : ApplicationService, IAdminSeatMapAppService
    {
        private readonly IRepository<Entities.SeatMap, Guid> _seatMapRepository;
        private readonly IRepository<Cinema, Guid> _cinemaRepository;
        private readonly IGmt7Clock _gmt7Clock;

        public SeatMapAppService(
            IRepository<Entities.SeatMap, Guid> seatMapRepository,
            IRepository<Cinema, Guid> cinemaRepository,
            IGmt7Clock gmt7Clock)
        {
            _seatMapRepository = seatMapRepository;
            _cinemaRepository = cinemaRepository;
            _gmt7Clock = gmt7Clock;
        }

        public async Task<PagedResultDto<SeatMapOutputDto>> GetSeatMapListAsync(Guid cinemaId, GetSeatMapListInputDto input)
        {
            await EnsureManagerOwnsCinemaAsync(cinemaId);

            var query = await _seatMapRepository.GetQueryableAsync();
            query = query.Where(x => x.CinemaId == cinemaId);

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x =>
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToListAsync();

            return new PagedResultDto<SeatMapOutputDto>(
                totalCount,
                items.Select(MapToOutput).ToList()
            );
        }

        public async Task<SeatMapOutputDto> GetSeatMapAsync(Guid id)
        {
            var entity = await _seatMapRepository.GetAsync(id);
            await EnsureManagerOwnsCinemaAsync(entity.CinemaId);
            return MapToOutput(entity);
        }

        public async Task<SeatMapOutputDto> CreateSeatMapAsync(Guid cinemaId, CreateSeatMapInputDto input)
        {
            await EnsureManagerOwnsCinemaAsync(cinemaId);
            ValidateSeatMapInput(input.Name, input.SeatCount, input.SeatLayout);

            var entity = new Entities.SeatMap(GuidGenerator.Create())
            {
                TenantId = CurrentTenant.Id,
                CinemaId = cinemaId,
                Name = input.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
                SeatLayout = input.SeatLayout,
                SeatCount = input.SeatCount,
                CreatedAt = _gmt7Clock.Gmt7Now,
                UpdatedAt = _gmt7Clock.Gmt7Now
            };

            entity = await _seatMapRepository.InsertAsync(entity, autoSave: true);
            return MapToOutput(entity);
        }

        public async Task<SeatMapOutputDto> UpdateSeatMapAsync(Guid id, UpdateSeatMapInputDto input)
        {
            var entity = await _seatMapRepository.GetAsync(id);
            await EnsureManagerOwnsCinemaAsync(entity.CinemaId);
            ValidateSeatMapInput(input.Name, input.SeatCount, input.SeatLayout);

            entity.Name = input.Name.Trim();
            entity.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
            entity.SeatLayout = input.SeatLayout;
            entity.SeatCount = input.SeatCount;
            entity.UpdatedAt = _gmt7Clock.Gmt7Now;

            entity = await _seatMapRepository.UpdateAsync(entity, autoSave: true);
            return MapToOutput(entity);
        }

        public async Task DeleteSeatMapAsync(Guid id)
        {
            var entity = await _seatMapRepository.GetAsync(id);
            await EnsureManagerOwnsCinemaAsync(entity.CinemaId);
            await _seatMapRepository.DeleteAsync(id, autoSave: true);
        }

        private static SeatMapOutputDto MapToOutput(Entities.SeatMap entity)
        {
            return new SeatMapOutputDto
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                CinemaId = entity.CinemaId,
                Name = entity.Name,
                Description = entity.Description,
                SeatLayout = entity.SeatLayout,
                SeatCount = entity.SeatCount,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private async Task EnsureManagerOwnsCinemaAsync(Guid cinemaId)
        {
            var cinema = await _cinemaRepository.FindAsync(cinemaId);
            if (cinema == null)
            {
                throw new UserFriendlyException("Cinema not found.");
            }

            if (CurrentUser.Id.HasValue && cinema.ManagerUserId.HasValue && cinema.ManagerUserId != CurrentUser.Id)
            {
                throw new UserFriendlyException("You can only manage seat maps in your assigned cinema.");
            }
        }

        private static void ValidateSeatMapInput(string? name, int seatCount, string? seatLayout)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new UserFriendlyException("Seat map name is required.");
            }

            if (seatCount <= 0)
            {
                throw new UserFriendlyException("Seat count must be a positive value.");
            }

            if (!string.IsNullOrWhiteSpace(seatLayout))
            {
                try
                {
                    _ = JsonDocument.Parse(seatLayout);
                }
                catch
                {
                    throw new UserFriendlyException("Seat layout JSON is invalid.");
                }
            }
        }
    }
}
