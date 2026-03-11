using System.Threading.Tasks;

namespace LTC.AdministrationService.Data;

public interface IAdministrationServiceDbSchemaMigrator
{
    Task MigrateAsync();
}
