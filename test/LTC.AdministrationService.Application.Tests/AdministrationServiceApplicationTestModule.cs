using Volo.Abp.Modularity;

namespace LTC.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceApplicationModule),
    typeof(AdministrationServiceDomainTestModule)
)]
public class AdministrationServiceApplicationTestModule : AbpModule
{

}
