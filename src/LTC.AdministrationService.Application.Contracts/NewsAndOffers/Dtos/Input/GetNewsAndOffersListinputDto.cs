using LTC.Shared.CrossCuttingConcerns.Pagination;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.NewsAndOffers.Dtos.Input
{
    public class GetNewsAndOffersListinputDto : PaginationWithSearchRequestDto
    {
        [MaxLength(64)]
        public string? Status { get; set; }
    }
}
