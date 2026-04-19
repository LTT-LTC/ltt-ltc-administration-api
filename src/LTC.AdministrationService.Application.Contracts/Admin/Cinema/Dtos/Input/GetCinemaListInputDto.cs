using LTC.Shared.CrossCuttingConcerns.Pagination;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.Cinemas.Dtos.Input
{
    public class GetCinemasListInputDto : PaginationWithSearchRequestDto
    {
        [MaxLength(64)]
        public string? Status { get; set; }
    }
}
