using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LTC.AdministrationService.EntityFrameworkCore;

public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        if (context is AdministrationServiceDbContext tenantContext)
        {
            // The cache key now includes the schema name.
            // This ensures EF Core recreates/caches the model for each schema.
            return (context.GetType(), tenantContext.GetCurrentSchema(), designTime);
        }

        return (context.GetType(), designTime);
    }
}
