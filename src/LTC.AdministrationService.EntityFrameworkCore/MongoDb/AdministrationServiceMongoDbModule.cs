using LTC.AdministrationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Configuration;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace LTC.CustomerManagement.MongoDb
{
    [DependsOn(
        typeof(AdministrationServiceDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class AdministrationServiceMongoDbModule : AbpModule
    {
        public static readonly ILoggerFactory DbLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<AdministrationServiceMongoDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            var loggerFactory = LoggerFactory.Create(b =>
            {
                b.AddSimpleConsole();
                b.SetMinimumLevel(LogLevel.Debug);
            });

            Configure<AbpMongoDbContextOptions>(options =>
            {
                options.MongoClientSettingsConfigurer = settings =>
                {
                    settings.LoggingSettings = new LoggingSettings(loggerFactory);
                };
            });
        }
    }
}
