using LTC.CustomerManagement.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Studio;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace LTC.AdministrationService.EntityFrameworkCore;

[DependsOn(
    typeof(AdministrationServiceDomainModule),
    //typeof(AdministrationServiceMongoDbModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
    )]
public class AdministrationServiceEntityFrameworkCoreModule : AbpModule
{
    public static readonly ILoggerFactory DbLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AbpIdentityDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpPermissionManagementDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpSettingManagementDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpTenantManagementDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpBackgroundJobsDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpAuditLoggingDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpFeatureManagementDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpOpenIddictDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AbpBlobStoringDatabaseDbProperties.DbSchema = AdministrationServiceConsts.DbSchema;
        AdministrationServiceEfCoreEntityExtensionMappings.Configure();  
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<AdministrationServiceDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        if (AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            return;
        }

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also AdministrationServiceDbContextFactory for EF Core tooling. */

            //options.UseSqlServer();
            options.Configure(context =>
            {
                context.DbContextOptions.UseLoggerFactory(DbLoggerFactory);
                context.DbContextOptions.EnableDetailedErrors();
                context.DbContextOptions.EnableSensitiveDataLogging();
                // context.DbContextOptions.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                context.UseSqlServer();
            });

        });
        
    }
}
