using LTC.AdministrationService.Events;
using LTC.AdministrationService.Localization;
using LTC.AdministrationService.MailTemplate;
using LTC.AdministrationService.MailTemplate.Dtos.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Auth.LocalEvents
{
    public class SendOTPEventHandler(
        IMailTemplateAppService mailTemplateAppService,
        IdentityUserManager identityUserManager,
        IStringLocalizer<AdministrationServiceResource> localizer,
        ICurrentTenant currentTenant,
        IConfiguration configuration
        ) : ILocalEventHandler<SendOTPEvent>, ITransientDependency
    {
        public async Task HandleEventAsync(SendOTPEvent eventData)
        {
            if (eventData != null)
            {
                var otpCode = GenerateSecure6DigitCode();
                var isEnableSettingsOTP = configuration.GetSection("OTP").GetValue<bool>("Enabled");
                var otpExpirationTime = (eventData.ExpireSeconds / 60).ToString();

                if (isEnableSettingsOTP)
                {
                    var defaultOtp = configuration.GetSection("OTP").GetValue<string>("OTPDefault");
                    otpExpirationTime = configuration.GetSection("OTP").GetValue<string>("Expiration");

                    otpExpirationTime = (Int32.Parse(otpExpirationTime ?? "0") / 60).ToString(); // Convert seconds to minutes
                    otpCode = (defaultOtp ?? string.Empty).ToString();
                }

                var user = await identityUserManager.FindByIdAsync(eventData.UserId.ToString())
                    ?? throw new UserFriendlyException(localizer["UserNotFound"]);

                await mailTemplateAppService.SendEmailAsync(new MailTemplateInputDto
                {
                    CcMail = new List<string> { user.Email },
                    EmailInfo = new EmailInfo
                    {
                        MailTemplateCode = MailTemplateConsts.SEND_OTP_CODE,
                        DataDictionary = new Dictionary<string, string>
                        {
                            { $"{MailTemplateConsts.User}.{MailTemplateConsts.Name}", user.Name },
                            { $"{MailTemplateConsts.OTP}.{MailTemplateConsts.OTPCode}", otpCode },
                            { $"{MailTemplateConsts.OTP}.{MailTemplateConsts.ExpireMinutes}", otpExpirationTime ?? string.Empty },
                            { $"{MailTemplateConsts.User}.{MailTemplateConsts.TenantName}", (currentTenant.Name ?? string.Empty).ToUpper() }
                        }
                    }
                });
            }
        }

        public static string GenerateSecure6DigitCode()
        {
            int value = RandomNumberGenerator.GetInt32(0, 1000000);
            return value.ToString("D6");
        }
    }
}
