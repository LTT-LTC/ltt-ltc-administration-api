using LTC.AdministrationService.Localization;
using LTC.Shared.CrossCuttingConcerns.Dtos.Pagination;
using LTC.Shared.CrossCuttingConcerns.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace LTC.AdministrationService.Employee.Dtos.Input
{
    public class GetListEmployeeInputDto : PaginationWithSearchRequestDto
    {
        public bool? IsActive { get; set; }
    }

    public class GetListEmployeeInputDtoValidator : AbstractValidator<GetListEmployeeInputDto>
    {
        
    }
}
