using LTC.AdministrationService;
using LTC.AdministrationService.Entities.Mongo;
using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace LTC.CustomerManagement.MongoDb
{
    public static class AdministrationServiceMongoDbContextExtensions
    {
        public static void ConfigureCollections(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions>? optionsAction = null
            )
        {
            var options = new AppMongoModelBuilderConfigurationOptions(AdministrationServiceConsts.MongoDbCollectionPrefix);

            Check.NotNull(builder, nameof(builder));
            builder.Entity<MailTemplate>(b =>
            {
                b.CollectionName = $"{AdministrationServiceConsts.MongoDbCollectionPrefix}.mail_template";
            });

            optionsAction?.Invoke(options);
        }
    }
}
