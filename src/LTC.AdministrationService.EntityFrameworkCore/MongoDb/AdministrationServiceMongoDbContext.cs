using LTC.AdministrationService;
using LTC.AdministrationService.Entities.Mongo;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace LTC.CustomerManagement.MongoDb
{
    [ConnectionStringName(AdministrationServiceConsts.MongoDbConnectionStringName)]
    public class AdministrationServiceMongoDbContext : AbpMongoDbContext
    {
        public IMongoCollection<MailTemplate> MailTemplates => Collection<MailTemplate>();
        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);
            modelBuilder.ConfigureCollections();
        }
    }
}
