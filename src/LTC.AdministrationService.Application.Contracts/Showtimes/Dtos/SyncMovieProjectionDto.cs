using System;

namespace LTC.AdministrationService.Showtimes.Dtos
{
    /// <summary>
    /// Replicated movie read-model payload pushed from Movie service.
    /// </summary>
    public class SyncMovieProjectionDto
    {
        public Guid MovieId { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
