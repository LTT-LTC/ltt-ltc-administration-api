using LTC.AdministrationService.Localization;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace LTC.AdministrationService.Employee.Dtos.Input
{
    public class UpdateEmployeeInputDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? OtherEmail { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public Guid? CinemaId { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Role { get; set; }
    }

    public class UpdateEmployeeInputValidator : AbstractValidator<UpdateEmployeeInputDto>
    {
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin",
            "Manager",
            "Staff",
            "POS"
        };

        public UpdateEmployeeInputValidator(IStringLocalizer<AdministrationServiceResource> localizer)
        {
            RuleFor(x => x.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:Name"]));

            RuleFor(x => x.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:Email"]));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["InvalidValue"], localizer["User:Email"]));

            RuleFor(x => x.PhoneNumber)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:PhoneNumber"]));

            RuleFor(x => x.Code)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:Code"]));

            RuleFor(x => x.Role)
                .Must(role => string.IsNullOrWhiteSpace(role) || AllowedRoles.Contains(role))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["InvalidValue"], localizer["User:Role"]));

            RuleFor(x => x.CinemaId)
                .NotNull()
                .When(x => string.Equals(x.Role, "Manager", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(x.Role, "Staff", StringComparison.OrdinalIgnoreCase))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["Cinema"]));
        }
    }
}
