using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities.Mongo
{
    public class MailTemplate : FullAuditedEntity<Guid>//, IMultiTenant
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
