using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
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
    private readonly IGmt7Clock _gmt7Clock;

    public ScreenAppService(
        IRepository<Screen, Guid> screenRepository,
        IRepository<SeatMap, Guid> seatMapRepository,
        IGmt7Clock gmt7Clock)
    {
        _screenRepository = screenRepository;
        _seatMapRepository = seatMapRepository;
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
        var seatMap = new SeatMap(GuidGenerator.Create())
        {
            TenantId = CurrentTenant.Id,
            CinemaId = cinemaId,
            SeatLayout = input.SeatLayout,
            SeatCount = input.SeatCount,
            CreatedAt = _gmt7Clock.Gmt7Now,
        };
        await _seatMapRepository.InsertAsync(seatMap, autoSave: true);

        var screen = ObjectMapper.Map<CreateScreenInputDto, Screen>(input);
        screen.CinemaId = cinemaId;
        screen.SeatMapId = seatMap.Id;
        screen.CreatedAt = _gmt7Clock.Gmt7Now;
        screen.TenantId = CurrentTenant.Id;

        screen = await _screenRepository.InsertAsync(screen, autoSave: true);
        return ToOutputDto(screen, seatMap);
    }

    public async Task<ScreenOutputDto> UpdateAsync(Guid id, UpdateScreenInputDto input)
    {
        var screen = await _screenRepository.GetAsync(id);

        ObjectMapper.Map(input, screen);
        screen.UpdatedAt = _gmt7Clock.Gmt7Now;

        if (screen.SeatMapId.HasValue)
        {
            var seatMap = await _seatMapRepository.FindAsync(screen.SeatMapId.Value);
            if (seatMap != null)
            {
                seatMap.SeatLayout = input.SeatLayout;
                seatMap.SeatCount = input.SeatCount;
                seatMap.UpdatedAt = _gmt7Clock.Gmt7Now;
                await _seatMapRepository.UpdateAsync(seatMap, autoSave: true);
            }
        }
        else
        {
            var seatMap = new SeatMap(GuidGenerator.Create())
            {
                TenantId = CurrentTenant.Id,
                CinemaId = screen.CinemaId,
                SeatLayout = input.SeatLayout,
                SeatCount = input.SeatCount,
                CreatedAt = _gmt7Clock.Gmt7Now,
            };
            await _seatMapRepository.InsertAsync(seatMap, autoSave: true);
            screen.SeatMapId = seatMap.Id;
        }

        screen = await _screenRepository.UpdateAsync(screen, autoSave: true);

        SeatMap? mapForDto = screen.SeatMapId.HasValue
            ? await _seatMapRepository.FindAsync(screen.SeatMapId.Value)
            : null;

        return ToOutputDto(screen, mapForDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        var screen = await _screenRepository.GetAsync(id);
        var seatMapId = screen.SeatMapId;

        await _screenRepository.DeleteAsync(id, autoSave: true);

        if (seatMapId.HasValue)
        {
            await _seatMapRepository.DeleteAsync(seatMapId.Value, autoSave: true);
        }
    }

    private ScreenOutputDto ToOutputDto(Screen screen, SeatMap? seatMap)
    {
        var dto = ObjectMapper.Map<Screen, ScreenOutputDto>(screen);
        dto.SeatLayout = seatMap?.SeatLayout;
        dto.SeatCount = seatMap?.SeatCount ?? 0;
        return dto;
    }
}
