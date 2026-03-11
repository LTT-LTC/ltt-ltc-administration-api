using LTC.AdministrationService.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace LTC.AdministrationService.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceApplicationContractsModule)
)]
public class AdministrationServiceDbMigratorModule : AbpModule
{
}