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
        IRepository<Cinema, Guid> cinemaRepository,
        IGmt7Clock gmt7Clock)
    {
        _screenRepository = screenRepository;
        _cinemaRepository = cinemaRepository;
        _gmt7Clock = gmt7Clock;
    }

    public async Task<PagedResultDto<ScreenOutputDto>> GetScreenListAsync(Guid cinemaId, GetScreenListInputDto input)
    {
        var screensQ = await _screenRepository.GetQueryableAsync();
        var query = screensQ.Where(s => s.CinemaId == cinemaId);

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x =>
                x.ScreenType != null && x.ScreenType.Contains(input.Keyword));
        }

        if (!string.IsNullOrWhiteSpace(input.Status))
        {
            query = query.Where(x => x.Status == input.Status);
        }

        var totalCount = await query.CountAsync();
        // Use long arithmetic to prevent integer overflow with large page numbers
        var skipCount = (long)(input.Page - 1) * input.Fetch;
        var page = await query
            .OrderBy(x => x.ScreenNumber)
            .Skip((int)skipCount)
            .Take(input.Fetch)
            .ToListAsync();

        var items = page.Select(ToOutputDto).ToList();

        return new PagedResultDto<ScreenOutputDto>(totalCount, items);
    }

    public async Task<ScreenOutputDto> GetScreenAsync(Guid id)
    {
        var screen = await _screenRepository.GetAsync(id);
        return ToOutputDto(screen);
    }

    public async Task<ScreenOutputDto> CreateScreenAsync(Guid cinemaId, CreateScreenInputDto input)
    {
        ValidateSeatLayoutInput(input.SeatLayout, input.SeatCount);
        await ValidateScreenInputAsync(cinemaId, input.ScreenNumber, input.ScreenType, input.SeatCount, input.Status, null);

        var screen = ObjectMapper.Map<CreateScreenInputDto, Screen>(input);
        screen.CinemaId = cinemaId;
        screen.SeatLayout = input.SeatLayout;
        screen.SeatCount = input.SeatCount;
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
        return ToOutputDto(screen);
    }

    public async Task<ScreenOutputDto> UpdateScreenAsync(Guid id, UpdateScreenInputDto input)
    {
        var screen = await _screenRepository.GetAsync(id);
        ValidateSeatLayoutInput(input.SeatLayout, input.SeatCount);
        await ValidateScreenInputAsync(screen.CinemaId, input.ScreenNumber, input.ScreenType, input.SeatCount, input.Status, id);

        ObjectMapper.Map(input, screen);
        screen.UpdatedAt = _gmt7Clock.Gmt7Now;
        screen.SeatLayout = input.SeatLayout;
        screen.SeatCount = input.SeatCount;

        try
        {
            screen = await _screenRepository.UpdateAsync(screen, autoSave: true);
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException($"Screen update failed: {ex.GetBaseException().Message}");
        }

        return ToOutputDto(screen);
    }

    public async Task DeleteScreenAsync(Guid id)
    {
        await _screenRepository.DeleteAsync(id, autoSave: true);
    }

    private ScreenOutputDto ToOutputDto(Screen screen)
    {
        return ObjectMapper.Map<Screen, ScreenOutputDto>(screen);
    }

    private static void ValidateSeatLayoutInput(string? seatLayout, int seatCount)
    {
        if (string.IsNullOrWhiteSpace(seatLayout))
        {
            throw new UserFriendlyException("Seat layout is required.");
        }

        try
        {
            _ = JsonDocument.Parse(seatLayout);
        }
        catch
        {
            throw new UserFriendlyException("Seat layout JSON is invalid.");
        }

        if (seatCount <= 0)
        {
            throw new UserFriendlyException("Seat count must be a positive value.");
        }
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
