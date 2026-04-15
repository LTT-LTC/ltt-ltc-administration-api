using System;
using System.Collections.Generic;
using System.Text;

namespace LTC.AdministrationService.NewsAndOffers.Dtos.Input
{
    public class UpdateNewsAndOffersDto
    {
        public Guid CinemaId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? PosterUrl { get; set; }
    }
}
