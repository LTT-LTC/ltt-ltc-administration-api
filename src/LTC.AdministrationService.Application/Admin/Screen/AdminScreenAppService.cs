using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using LTC.AdministrationService.Admin.Screen.Dtos.Input;
using LTC.AdministrationService.Admin.Screen.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.Pagination;

namespace LTC.AdministrationService.Admin.Screen
{
    public class AdminScreenAppService : ApplicationService, IAdminScreenAppService
    {
        private readonly IRepository<Entities.Screen, Guid> _screenRepository;

        public AdminScreenAppService(IRepository<Entities.Screen, Guid> screenRepository)
        {
            _screenRepository = screenRepository;
        }

        public async Task<PagedResultDto<ScreenOutputDto>> GetListAsync(Guid cinemaId, GetScreenListInputDto input)
        {
            var queryable = await _screenRepository.GetQueryableAsync();
            
            queryable = queryable.Where(x => x.CinemaId == cinemaId);
            
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                queryable = queryable.Where(x => x.ScreenType != null && x.ScreenType.Contains(input.Keyword));
            }

            if (!string.IsNullOrWhiteSpace(input.Status))
            {
                queryable = queryable.Where(x => x.Status == input.Status);
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderBy(x => x.ScreenNumber)
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToListAsync();

            return new PagedResultDto<ScreenOutputDto>(
                totalCount,
                items.Select(x => ObjectMapper.Map<Entities.Screen, ScreenOutputDto>(x)).ToList()
            );
        }

        public async Task<ScreenOutputDto> GetAsync(Guid id)
        {
            var screen = await _screenRepository.GetAsync(id);
            return ObjectMapper.Map<Entities.Screen, ScreenOutputDto>(screen);
        }

        public async Task<ScreenOutputDto> CreateAsync(Guid cinemaId, CreateScreenInputDto input)
        {
            var screen = ObjectMapper.Map<CreateScreenInputDto, Entities.Screen>(input);
            screen.CinemaId = cinemaId;
            screen.CreatedAt = DateTime.UtcNow;
            
            screen = await _screenRepository.InsertAsync(screen, autoSave: true);
            return ObjectMapper.Map<Entities.Screen, ScreenOutputDto>(screen);
        }

        public async Task<ScreenOutputDto> UpdateAsync(Guid id, UpdateScreenInputDto input)
        {
            var screen = await _screenRepository.GetAsync(id);
            ObjectMapper.Map(input, screen);
            screen.UpdatedAt = DateTime.UtcNow;
            
            screen = await _screenRepository.UpdateAsync(screen, autoSave: true);
            return ObjectMapper.Map<Entities.Screen, ScreenOutputDto>(screen);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _screenRepository.DeleteAsync(id, autoSave: true);
        }
    }
}
