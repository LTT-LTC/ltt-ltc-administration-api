namespace LTC.AdministrationService.Customer.Cinemas.Dtos.Input
{
    public class GetCinemaCustomerListInputDto
    {
        public string? Keyword { get; set; }
        public string? City { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
