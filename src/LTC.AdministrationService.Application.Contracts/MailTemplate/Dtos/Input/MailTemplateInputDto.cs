using System.Collections.Generic;

namespace LTC.AdministrationService.MailTemplate.Dtos.Input
{
    public class MailTemplateInputDto
    {
        public List<string> ToMail { get; set; } = new();
        public List<string> CcMail { get; set; } = new();
        public List<string> BccMail { get; set; } = new();
        public EmailInfo? EmailInfo { get; set; }
    }

    public class EmailInfo
    {
        public string MailTemplateCode { get; set; }
        public Dictionary<string, string> DataDictionary { get; set; }
    }
}
