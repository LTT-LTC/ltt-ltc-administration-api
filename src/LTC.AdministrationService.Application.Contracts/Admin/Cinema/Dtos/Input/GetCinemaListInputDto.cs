using LTC.Shared.CrossCuttingConcerns.Pagination;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.Cinema.Dtos.Input
{
    public class GetCinemaListInputDto : PaginationWithSearchRequestDto
    {
        [MaxLength(64)]
        public string? Status { get; set; }
    }
}
