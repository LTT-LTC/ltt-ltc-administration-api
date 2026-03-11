using LTC.AdministrationService.MailTemplate.Dtos.Input;
using LTC.AdministrationService.MailTemplate.Extensions;
using LTC.CustomerManagement.Settings;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Emailing;
using Volo.Abp.Uow;

namespace LTC.AdministrationService.MailTemplate
{
    public class MailTemplateAppService : AdministrationServiceAppService, IMailTemplateAppService
    {
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IMailTemplateRepository _mailTemplateRepository;
        private readonly IConfiguration _configuration;

        public MailTemplateAppService(
            IEmailSender emailSender,
            IUnitOfWorkManager unitOfWorkManager,
            IMailTemplateRepository mailTemplateRepository,
            IConfiguration configuration
            )
        {
            _emailSender = emailSender;
            _unitOfWorkManager = unitOfWorkManager;
            _mailTemplateRepository = mailTemplateRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        /// <exception cref="BusinessException"></exception>
        public async Task<bool> SendEmailAsync(MailTemplateInputDto input)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var isTesting = _configuration.GetSection("Settings").GetValue<bool>("Abp.Mailing.IsTesting");
                var mailTemplateQueryable = await _mailTemplateRepository.GetQueryableAsync();

                if (input.EmailInfo == null)
                    throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["InvalidValue"], L["EmailInfo"]));

                // kiểm tra danh sách tất cả mail có bị rỗng không
                if (!input.ToMail.Any() &&
                    !input.CcMail.Any() &&
                    !input.BccMail.Any())
                    throw new BusinessException(CommonExtensions.GetValidateMessage(L["InvalidValue"], L["EmailInfo"]));

                // lấy thông tin mail template
                var mailTemplate = mailTemplateQueryable.FirstOrDefault(x => x.Code == input.EmailInfo.MailTemplateCode)
                    ?? throw new UserFriendlyException(CommonExtensions.GetValidateMessage(L["NotFound"], L["MailTemplate"]));
                
                var body = string.Empty;
                var subject = string.Empty;

                if (isTesting)
                {
                    body += $"to: {string.Join(", ", input.ToMail)}<br>cc: {string.Join(", ",input.CcMail)}<br>bcc: {string.Join(", ",input.BccMail)}<br>";
                }

                // map data cho subject và body
                subject = ReplaceMailTemplateVariable(input.EmailInfo.DataDictionary, mailTemplate.Subject);
                body += ReplaceMailTemplateVariable(input.EmailInfo.DataDictionary, mailTemplate.Body);

                // placeholder cho mail
                // Example: ##UserName##, ##ResetLink##

                var emailMessage = new MailMessage
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body
                };

                // nếu đang test mail thì gửi về mail test
                if (isTesting)
                {
                    var testToEmail = _configuration.GetSection("Settings").GetValue<string>("Abp.Mailing.ToMailTesting");
                    var testCcEmail = _configuration.GetSection("Settings").GetValue<string>("Abp.Mailing.CcMailTesting");
                    var testBccEmail = _configuration.GetSection("Settings").GetValue<string>("Abp.Mailing.BccMailTesting");

                    var splitedTestToEmails = (testToEmail ?? string.Empty).Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    var splitedTestCcEmails = (testCcEmail ?? string.Empty).Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    var splitedTestBccEmails = (testBccEmail ?? string.Empty).Split(',', System.StringSplitOptions.RemoveEmptyEntries).ToList();

                    emailMessage.To.AddRange(splitedTestToEmails);
                    emailMessage.CC.AddRange(splitedTestCcEmails);
                    emailMessage.Bcc.AddRange(splitedTestBccEmails);

                    await _emailSender.SendAsync(emailMessage);
                    await uow.CompleteAsync();
                    return true;
                }

                emailMessage.To.AddRange(input.ToMail);
                emailMessage.CC.AddRange(input.CcMail);
                emailMessage.Bcc.AddRange(input.BccMail);

                await _emailSender.SendAsync(emailMessage);
                await uow.CompleteAsync();
                return true;
            }
        }

        public string ReplaceMailTemplateVariable(Dictionary<string, string> data, string content, string shortCode = "##")
        {
            foreach (KeyValuePair<string, string> item in data)
            {
                content = content.Replace($"{shortCode}{item.Key}{shortCode}", HtmlUtilities.ClearFormatHtml(item.Value));
            }
            return content;
        }
    }
}
