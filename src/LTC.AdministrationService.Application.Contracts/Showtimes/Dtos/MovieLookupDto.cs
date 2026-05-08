using System;
using System.Collections.Generic;

namespace LTC.AdministrationService.Showtimes.Dtos
{
    /// <summary>
    /// Snapshot of a Movie returned by the movie-service public detail endpoint.
    /// Embedded as a virtual object on showtime DTOs so callers don't have to issue
    /// a second request just to render the basic movie details.
    /// </summary>
    public class MovieLookupDto
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? OriginalTitle { get; set; }
        public int? DurationMins { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PremiereDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public Guid? StudioId { get; set; }
        public string? StudioName { get; set; }
        public Guid? RatingId { get; set; }
        public string? RatingCode { get; set; }
        public string? RatingName { get; set; }
        public MovieLookupStudioDto? Studio { get; set; }
        public List<string>? GenreNames { get; set; }
        public List<MovieLookupGenreDto>? Genres { get; set; }
        public List<MovieLookupActorRoleDto>? ActorRoles { get; set; }
        public List<MovieLookupCastDto>? Cast { get; set; }
    }

    public class MovieLookupStudioDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MovieLookupGenreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MovieLookupActorRoleDto
    {
        public string ActorName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class MovieLookupCastDto
    {
        public MovieLookupActorDto? Actor { get; set; }
        public MovieLookupRoleDto? Role { get; set; }
        public string? CharacterName { get; set; }
        public string? ActorName { get; set; }
        public string? RoleName { get; set; }
    }

    public class MovieLookupActorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MovieLookupRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
