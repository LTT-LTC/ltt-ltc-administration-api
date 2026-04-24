using LTC.AdministrationService.Localization;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Volo.Abp.Identity;

namespace LTC.AdministrationService.Employee.Dtos.Input
{
    public class CreateEmployeeInputDto
    {
        /// <summary>
        /// Tên nhân sự
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mã nhân sự
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Email nhân sự
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Email khác của nhân sự
        /// </summary>
        public string? OtherEmail { get; set; }

        /// <summary>
        /// Số điện thoại nhân sự
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Rạp của nhân sự
        /// </summary>
        public Guid? CinemaId { get; set; }

        /// <summary>
        /// Ngày tuyển dụng
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Role được gán trong ABP Identity (Admin/Manager/Staff/POS)
        /// </summary>
        public string Role { get; set; } = "Staff";
    }

    public class CreateEmployeeInputValidator : AbstractValidator<CreateEmployeeInputDto>
    {
        private readonly IIdentityUserAppService _userAppService;
        private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin",
            "Manager",
            "Staff",
            "POS"
        };

        public CreateEmployeeInputValidator(IStringLocalizer<AdministrationServiceResource> localizer, IIdentityUserAppService userAppService)
        {
            _userAppService = userAppService;

            // Name
            RuleFor(x => x.Name)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:Name"]));

            RuleFor(x => x.Name)
                .MaximumLength(ValidationConsts.MediumInputMaxLength)
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["InvalidValue"], localizer["User:JoinedDate"]));

            // Code
            RuleFor(x => x.Code)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:Code"]));

            // Email
            RuleFor(x => x.Email)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:Email"]));
           
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["InvalidValue"], localizer["User:Email"]));

            RuleFor(x => x.Email)
                .MaximumLength(ValidationConsts.SmallInputMaxLength)
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["MaxLength"], localizer["User:Email"], ValidationConsts.SmallInputMaxLength));

            RuleFor(x => x.Email)
                .MustAsync(async (email, cancellation) =>
                {
                    try
                    {
                        var existingUser = await _userAppService.FindByEmailAsync(email);
                        return existingUser == null;
                    }
                    catch
                    {
                        return true;
                    }
                })
                .WithMessage(CommonExtensions.GetValidateMessage(
                    localizer["Duplicate"],
                    localizer["User:Email"])
                );

            // other email
            RuleFor(x => x.OtherEmail)
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.OtherEmail))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["InvalidValue"], localizer["User:OtherEmail"]));

            RuleFor(x => x.OtherEmail)
                .MaximumLength(ValidationConsts.SmallInputMaxLength)
                .When(x => !string.IsNullOrEmpty(x.OtherEmail))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["MaxLength"], localizer["User:OtherEmail"], ValidationConsts.SmallInputMaxLength));

            RuleFor(x => x.OtherEmail)
                .MustAsync(async (email, cancellation) =>
                {
                    try
                    {
                        var existingUser = await _userAppService.FindByEmailAsync(email);
                        return existingUser == null;
                    }
                    catch
                    {
                        return true;
                    }
                })
                .When(x => !string.IsNullOrEmpty(x.OtherEmail))
                .WithMessage(CommonExtensions.GetValidateMessage(
                    localizer["Duplicate"],
                    localizer["User:OtherEmail"])
                );

            RuleFor(x => x.PhoneNumber)
                .Must(x => !string.IsNullOrEmpty(x))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["User:PhoneNumber"]));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\+?84|0)(3[2-9]|5[25689]|7[0|6-9]|8[1-9]|9[0-9])([0-9]{7})$", RegexOptions.IgnoreCase)
                .WithMessage(CommonExtensions.GetValidateMessage(
                    localizer["InvalidValue"],
                    localizer["User:PhoneNumber"])
                );

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(EmployeeConsts.PhoneNumberMaxLength)
                .WithMessage(CommonExtensions.GetValidateMessage(
                    localizer["MaxLength"],
                    localizer["User:PhoneNumber"],
                    EmployeeConsts.PhoneNumberMaxLength)
                );

            RuleFor(x => x.Role)
                .Must(role => !string.IsNullOrWhiteSpace(role) && AllowedRoles.Contains(role))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["InvalidValue"], localizer["User:Role"]));

            RuleFor(x => x.CinemaId)
                .NotNull()
                .When(x => string.Equals(x.Role, "Manager", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(x.Role, "Staff", StringComparison.OrdinalIgnoreCase))
                .WithMessage(CommonExtensions.GetValidateMessage(localizer["NotEmpty"], localizer["Cinema"]));
        }
    }
}