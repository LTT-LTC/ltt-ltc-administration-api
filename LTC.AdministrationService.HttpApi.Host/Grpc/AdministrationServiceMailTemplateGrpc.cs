//using AdministrationService.Protos;
//using LTC.AdministrationService.Localization;
//using LTC.AdministrationService.MailTemplate;
//using LTC.AdministrationService.MailTemplate.Dtos.Input;
//using Grpc.Core;
//using LTC.AdministrationService.Localization;
//using Microsoft.Extensions.Localization;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Volo.Abp.Uow;

//namespace LTC.AdministrationService.Grpc
//{
//    public class AdministrationServiceMailTemplateGrpc : AdministrationServiceMailTemplate.AdministrationServiceMailTemplateBase
//    {
//        private readonly IUnitOfWorkManager _unitOfWorkManager;
//        private readonly IStringLocalizer<AdministrationServiceResource> _localizer;
//        private readonly IMailTemplateAppService _mailTemplateAppService;

//        public AdministrationServiceMailTemplateGrpc(
//            IUnitOfWorkManager unitOfWorkManager,
//            IStringLocalizer<AdministrationServiceResource> localizer,
//            IMailTemplateAppService mailTemplateAppService
//            )
//        {
//            _unitOfWorkManager = unitOfWorkManager;
//            _localizer = localizer;
//            _mailTemplateAppService = mailTemplateAppService;
//        }

//        public override async Task<SendEmailResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
//        {
//            try
//            {
//                using (var uow = _unitOfWorkManager.Begin())
//                {
//                    var toMail = request.ToMail;
//                    var ccMail = request.CcMail;
//                    var bccMail = request.BccMail;
//                    var emailInfo = request.EmailInfo;

//                    var isEmailSent = await _mailTemplateAppService.SendEmailAsync(new MailTemplateInputDto
//                    {
//                        ToMail = toMail.ToList(),
//                        CcMail = ccMail.ToList(),
//                        BccMail = bccMail.ToList(),
//                        EmailInfo = new MailTemplate.Dtos.Input.EmailInfo
//                        {
//                            DataDictionary = emailInfo.MailData.ToDictionary(k => k.Key, v => v.Value),
//                            MailTemplateCode = emailInfo.MailTemplateCode
//                        }
//                    });

//                    var response = new SendEmailResponse();
//                    response.IsSuccess = isEmailSent;
//                    response.Data = isEmailSent;

//                    return response;
//                }
//            }
//            catch (Exception ex)
//            {
//                return new SendEmailResponse() { Error = ex.Message, IsSuccess = false };
//            }
//        }
//    }
//}
