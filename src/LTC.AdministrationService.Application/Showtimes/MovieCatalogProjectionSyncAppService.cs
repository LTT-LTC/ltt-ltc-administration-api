using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Showtimes.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace LTC.AdministrationService.Showtimes
{
    public class MovieCatalogProjectionSyncAppService : ApplicationService, IMovieCatalogProjectionSyncAppService
    {
        private readonly IRepository<MovieProjection, Guid> _movieProjectionRepository;
        private readonly IRepository<MovieDistributionProjection, Guid> _distributionProjectionRepository;

        public MovieCatalogProjectionSyncAppService(
            IRepository<MovieProjection, Guid> movieProjectionRepository,
            IRepository<MovieDistributionProjection, Guid> distributionProjectionRepository)
        {
            _movieProjectionRepository = movieProjectionRepository;
            _distributionProjectionRepository = distributionProjectionRepository;
        }

        public async Task UpsertMovieAsync(SyncMovieProjectionDto input)
        {
            var entity = await _movieProjectionRepository.FindAsync(input.MovieId);
            if (entity == null)
            {
                entity = new MovieProjection(input.MovieId)
                {
                    Title = input.Title,
                    Status = input.Status,
                    DurationInMinutes = input.DurationInMinutes,
                    CreatedAt = Clock.Now,
                    UpdatedAt = Clock.Now
                };

                await _movieProjectionRepository.InsertAsync(entity, true);
                return;
            }

            entity.Title = input.Title;
            entity.Status = input.Status;
            entity.DurationInMinutes = input.DurationInMinutes;
            entity.UpdatedAt = Clock.Now;
            await _movieProjectionRepository.UpdateAsync(entity, true);
        }

        public async Task DeleteMovieAsync(Guid movieId)
        {
            await _movieProjectionRepository.DeleteAsync(movieId);
        }

        public async Task UpsertDistributionAsync(SyncMovieDistributionProjectionDto input)
        {
            var entity = await _distributionProjectionRepository.FindAsync(input.DistributionId);
            if (entity == null)
            {
                entity = new MovieDistributionProjection(input.DistributionId)
                {
                    MovieId = input.MovieId,
                    Format = input.Format,
                    StartDate = input.StartDate,
                    EndDate = input.EndDate,
                    CreatedAt = Clock.Now,
                    UpdatedAt = Clock.Now
                };

                await _distributionProjectionRepository.InsertAsync(entity, true);
                return;
            }

            entity.MovieId = input.MovieId;
            entity.Format = input.Format;
            entity.StartDate = input.StartDate;
            entity.EndDate = input.EndDate;
            entity.UpdatedAt = Clock.Now;
            await _distributionProjectionRepository.UpdateAsync(entity, true);
        }

        public async Task DeleteDistributionAsync(Guid distributionId)
        {
            await _distributionProjectionRepository.DeleteAsync(distributionId);
        }
    }
}
