using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace LTC.CustomerManagement.MongoDb
{
    public class AppMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public AppMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}
