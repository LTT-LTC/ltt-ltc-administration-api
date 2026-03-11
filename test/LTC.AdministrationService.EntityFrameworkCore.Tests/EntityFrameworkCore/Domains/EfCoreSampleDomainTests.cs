using LTC.AdministrationService.Samples;
using Xunit;

namespace LTC.AdministrationService.EntityFrameworkCore.Domains;

[Collection(AdministrationServiceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AdministrationServiceEntityFrameworkCoreTestModule>
{

}
