using System;

namespace LTC.AdministrationService.Customer.Showtimes.Dtos.Input
{
    public class GetShowtimeCustomerListInputDto
    {
        public Guid? MovieId { get; set; }
        public Guid? CinemaId { get; set; }
        public DateTime? Date { get; set; }
    }
}
