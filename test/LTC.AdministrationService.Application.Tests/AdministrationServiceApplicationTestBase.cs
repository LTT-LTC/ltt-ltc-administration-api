using Volo.Abp.Modularity;

namespace LTC.AdministrationService;

public abstract class AdministrationServiceApplicationTestBase<TStartupModule> : AdministrationServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
