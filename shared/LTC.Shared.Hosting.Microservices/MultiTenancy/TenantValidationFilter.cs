using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace LTC.Shared.Hosting.Microservices.MultiTenancy
{
    public class TenantValidationFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly ITenantStore _tenantStore;

        public TenantValidationFilter(ITenantStore tenantStore)
        {
            _tenantStore = tenantStore;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            if (request.Headers.TryGetValue("X-Tenant", out var tenantHeader))
            {
                var tenantName = tenantHeader.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(tenantName))
                {
                    // Validate against ABP tenant store directly (same pattern as movie/customer hosts).
                    // Using ICurrentTenant.Id fails when X-Tenant is sent but CurrentTenant was not bound
                    // (e.g. anonymous routes or header vs resolved-tenant mismatch).
                    var tenantInfo = await _tenantStore.FindAsync(tenantName)
                        ?? await _tenantStore.FindAsync(tenantName.ToUpperInvariant());

                    if (tenantInfo == null)
                    {
                        throw new UserFriendlyException($"Tenant '{tenantName}' is invalid or could not be found.");
                    }
                }
            }

            await next();
        }
    }
}
