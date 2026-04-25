using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using LTC.AdministrationService.Admin.Screens;
using LTC.AdministrationService.Admin.Screens.Dtos.Input;
using LTC.AdministrationService.Admin.Screens.Dtos.Output;
using LTC.AdministrationService.Entities;
using LTC.Shared.Hosting.Microservices.Timing;

namespace LTC.AdministrationService.Screens;

public class ScreenAppService : ApplicationService, IAdminScreenAppService
{
    private readonly IRepository<Screen, Guid> _screenRepository;
    private readonly IRepository<SeatMap, Guid> _seatMapRepository;
    private readonly IRepository<Cinema, Guid> _cinemaRepository;
    private readonly IGmt7Clock _gmt7Clock;
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "active",
        "maintenance",
        "inactive"
    };

    public ScreenAppService(
        IRepository<Screen, Guid> screenRepository,
        IRepository<SeatMap, Guid> seatMapRepository,
        IRepository<Cinema, Guid> cinemaRepository,
        IGmt7Clock gmt7Clock)
    {
        _screenRepository = screenRepository;
        _seatMapRepository = seatMapRepository;
        _cinemaRepository = cinemaRepository;
        _gmt7Clock = gmt7Clock;
    }

    public async Task<PagedResultDto<ScreenOutputDto>> GetListAsync(Guid cinemaId, GetScreenListInputDto input)
    {
        var screensQ = await _screenRepository.GetQueryableAsync();
        var seatMapsQ = await _seatMapRepository.GetQueryableAsync();

        var joined =
            from s in screensQ
            where s.CinemaId == cinemaId
            join m in seatMapsQ on s.SeatMapId equals m.Id into mapJoin
            from m in mapJoin.DefaultIfEmpty()
            select new { Screen = s, SeatMap = m };

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            joined = joined.Where(x =>
                x.Screen.ScreenType != null && x.Screen.ScreenType.Contains(input.Keyword));
        }

        if (!string.IsNullOrWhiteSpace(input.Status))
        {
            joined = joined.Where(x => x.Screen.Status == input.Status);
        }

        var totalCount = await joined.CountAsync();
        var page = await joined
            .OrderBy(x => x.Screen.ScreenNumber)
            .Skip((input.Page - 1) * input.Fetch)
            .Take(input.Fetch)
            .ToListAsync();

        var items = page.Select(x => ToOutputDto(x.Screen, x.SeatMap)).ToList();

        return new PagedResultDto<ScreenOutputDto>(totalCount, items);
    }

    public async Task<ScreenOutputDto> GetAsync(Guid id)
    {
        var screen = await _screenRepository.GetAsync(id);
        SeatMap? seatMap = null;
        if (screen.SeatMapId.HasValue)
        {
            seatMap = await _seatMapRepository.FindAsync(screen.SeatMapId.Value);
        }

        return ToOutputDto(screen, seatMap);
    }

    public async Task<ScreenOutputDto> CreateAsync(Guid cinemaId, CreateScreenInputDto input)
    {
        var seatMap = await ValidateAndGetSeatMapAsync(cinemaId, input.SeatMapId);
        await ValidateScreenInputAsync(cinemaId, input.ScreenNumber, input.ScreenType, seatMap.SeatCount, input.Status, null);

        var screen = ObjectMapper.Map<CreateScreenInputDto, Screen>(input);
        screen.CinemaId = cinemaId;
        screen.SeatMapId = seatMap.Id;
        screen.SeatCount = seatMap.SeatCount;
        screen.CreatedAt = _gmt7Clock.Gmt7Now;
        screen.TenantId = CurrentTenant.Id;

        try
        {
            screen = await _screenRepository.InsertAsync(screen, autoSave: true);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"Screen create failed: {ex.GetBaseException().Message}");
        }
        return ToOutputDto(screen, seatMap);
    }

    public async Task<ScreenOutputDto> UpdateAsync(Guid id, UpdateScreenInputDto input)
    {
        var screen = await _screenRepository.GetAsync(id);
        var seatMap = await ValidateAndGetSeatMapAsync(screen.CinemaId, input.SeatMapId);
        await ValidateScreenInputAsync(screen.CinemaId, input.ScreenNumber, input.ScreenType, seatMap.SeatCount, input.Status, id);

        ObjectMapper.Map(input, screen);
        screen.UpdatedAt = _gmt7Clock.Gmt7Now;
        screen.SeatMapId = seatMap.Id;
        screen.SeatCount = seatMap.SeatCount;

        try
        {
            screen = await _screenRepository.UpdateAsync(screen, autoSave: true);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"Screen update failed: {ex.GetBaseException().Message}");
        }

        return ToOutputDto(screen, seatMap);
    }

    public async Task DeleteAsync(Guid id)
    {
        var screen = await _screenRepository.GetAsync(id);
        var seatMapId = screen.SeatMapId;

        await _screenRepository.DeleteAsync(id, autoSave: true);

    }

    private ScreenOutputDto ToOutputDto(Screen screen, SeatMap? seatMap)
    {
        var dto = ObjectMapper.Map<Screen, ScreenOutputDto>(screen);
        dto.SeatMapName = seatMap?.Name;
        dto.SeatMapDescription = seatMap?.Description;
        dto.SeatLayout = seatMap?.SeatLayout;
        dto.SeatCount = seatMap?.SeatCount ?? 0;
        return dto;
    }

    private async Task<SeatMap> ValidateAndGetSeatMapAsync(Guid cinemaId, Guid seatMapId)
    {
        if (seatMapId == Guid.Empty)
        {
            throw new UserFriendlyException("Seat map is required.");
        }

        var seatMap = await _seatMapRepository.FindAsync(seatMapId);
        if (seatMap == null)
        {
            throw new UserFriendlyException("Seat map not found.");
        }

        if (seatMap.CinemaId != cinemaId)
        {
            throw new UserFriendlyException("Selected seat map does not belong to this cinema.");
        }

        if (seatMap.SeatCount <= 0)
        {
            throw new UserFriendlyException("Seat map seat count must be a positive value.");
        }

        if (!string.IsNullOrWhiteSpace(seatMap.SeatLayout))
        {
            try
            {
                _ = JsonDocument.Parse(seatMap.SeatLayout);
            }
            catch
            {
                throw new UserFriendlyException("Seat map layout JSON is invalid.");
            }
        }

        return seatMap;
    }

    private async Task ValidateScreenInputAsync(
        Guid cinemaId,
        int screenNumber,
        string? screenType,
        int seatCount,
        string? status,
        Guid? editingId)
    {
        if (screenNumber <= 0)
        {
            throw new UserFriendlyException("Screen number must be a positive value.");
        }

        if (!string.IsNullOrWhiteSpace(status) && !AllowedStatuses.Contains(status))
        {
            throw new UserFriendlyException("Screen status is invalid.");
        }

        if (string.IsNullOrWhiteSpace(screenType))
        {
            throw new UserFriendlyException("Screen type is required.");
        }

        if (seatCount <= 0)
        {
            throw new UserFriendlyException("Seat count must be a positive value.");
        }

        var cinema = await _cinemaRepository.FindAsync(cinemaId);
        if (cinema == null)
        {
            throw new UserFriendlyException("Cinema not found.");
        }

        if (CurrentUser.Id.HasValue && cinema.ManagerUserId.HasValue && cinema.ManagerUserId != CurrentUser.Id)
        {
            throw new UserFriendlyException("You can only manage screens in your assigned cinema.");
        }

        var screensQ = await _screenRepository.GetQueryableAsync();
        var duplicate = await screensQ.AnyAsync(x =>
            x.CinemaId == cinemaId
            && x.ScreenNumber == screenNumber
            && (!editingId.HasValue || x.Id != editingId.Value));

        if (duplicate)
        {
            throw new UserFriendlyException("Screen number already exists in this cinema.");
        }
    }
}
