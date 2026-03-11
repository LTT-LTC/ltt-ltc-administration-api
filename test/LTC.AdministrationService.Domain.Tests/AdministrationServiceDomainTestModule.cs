using Volo.Abp.Modularity;

namespace LTC.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceDomainModule),
    typeof(AdministrationServiceTestBaseModule)
)]
public class AdministrationServiceDomainTestModule : AbpModule
{

}
