using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace LTC.AdministrationService.Data;

/* This is used if database provider does't define
 * IAdministrationServiceDbSchemaMigrator implementation.
 */
public class NullAdministrationServiceDbSchemaMigrator : IAdministrationServiceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
