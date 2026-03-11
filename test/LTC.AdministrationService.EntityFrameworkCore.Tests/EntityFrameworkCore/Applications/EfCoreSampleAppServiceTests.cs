using LTC.AdministrationService.Samples;
using Xunit;

namespace LTC.AdministrationService.EntityFrameworkCore.Applications;

[Collection(AdministrationServiceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AdministrationServiceEntityFrameworkCoreTestModule>
{

}
