using System;

namespace LTC.AdministrationService.Showtimes.Dtos
{
    /// <summary>
    /// Replicated movie distribution read-model payload pushed from Movie service.
    /// </summary>
    public class SyncMovieDistributionProjectionDto
    {
        public Guid DistributionId { get; set; }
        public Guid MovieId { get; set; }
        public string? Format { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
