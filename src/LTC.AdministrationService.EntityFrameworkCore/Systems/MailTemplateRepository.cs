using LTC.CustomerManagement.MongoDb;
using LTC.CustomerManagement.Settings;
using System;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LTC.AdministrationService.Systems
{
    public class MailTemplateRepository : MongoDbRepository<AdministrationServiceMongoDbContext, Entities.Mongo.MailTemplate, Guid>, IMailTemplateRepository
    {
        public MailTemplateRepository(IMongoDbContextProvider<AdministrationServiceMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
