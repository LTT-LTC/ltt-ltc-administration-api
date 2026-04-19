using System;
using LTC.Shared.CrossCuttingConcerns.Pagination;
using System.ComponentModel.DataAnnotations;

namespace LTC.AdministrationService.Admin.Screens.Dtos.Input
{
    public class GetScreenListInputDto : PaginationWithSearchRequestDto
    {
        public Guid CinemaId { get; set; }

        [MaxLength(64)]
        public string? Status { get; set; }
    }
}
