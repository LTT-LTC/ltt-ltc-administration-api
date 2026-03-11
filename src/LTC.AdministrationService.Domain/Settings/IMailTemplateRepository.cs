using LTC.AdministrationService.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LTC.CustomerManagement.Settings
{
    public interface IMailTemplateRepository : IRepository<MailTemplate, Guid>
    {
    }
}
