using LTC.AdministrationService.Events;
using LTC.AdministrationService.MailTemplate;
using LTC.AdministrationService.MailTemplate.Dtos.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.MultiTenancy;
using System;
using Volo.Abp.TenantManagement;

namespace LTC.AdministrationService.Auth.LocalEvents
{
    public class RecoveryPasswordEventHandler(
        IMailTemplateAppService mailTemplateAppService,
        ICurrentTenant currentTenant,
        ITenantRepository tenantRepository
        ) : ILocalEventHandler<RecoveryPasswordEvent>, ITransientDependency
    {
        public async Task HandleEventAsync(RecoveryPasswordEvent eventData)
        {
            if (eventData != null)
            {
                // Get tenant name from repository
                string tenantName = string.Empty;
                if (currentTenant.Id.HasValue)
                {
                    var tenant = await tenantRepository.FindAsync(currentTenant.Id.Value);
                    tenantName = tenant?.Name ?? string.Empty;
                }

                await mailTemplateAppService.SendEmailAsync(new MailTemplateInputDto
                {
                    ToMail = new List<string> { eventData.Email },
                    EmailInfo = new EmailInfo
                    {
                        MailTemplateCode = MailTemplateConsts.PASSWORD_RECOVERY,
                        DataDictionary = new Dictionary<string, string>
                        {
                            { $"{MailTemplateConsts.User}.{MailTemplateConsts.Name}", eventData.Name },
                            { $"{MailTemplateConsts.User}.{MailTemplateConsts.UserName}", eventData.UserName ?? string.Empty },
                            { $"{MailTemplateConsts.User}.{MailTemplateConsts.TenantName}", tenantName.ToUpper() },
                            { $"{MailTemplateConsts.PasswordReset}.{MailTemplateConsts.ResetLink}", eventData.ResetUrl ?? string.Empty },
                            { $"{MailTemplateConsts.PasswordReset}.{MailTemplateConsts.ExpireHours}", eventData.ExpireHours.ToString() ?? string.Empty }
                        }
                    }
                });
            }
        }
    }
}
