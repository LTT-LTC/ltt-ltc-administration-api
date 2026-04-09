using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Microsoft.AspNetCore.Identity;

namespace LTC.AdministrationService.Data;

public class AdministrationServiceInitialDataSeeder : IDataSeedContributor, ITransientDependency
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantManager _tenantManager;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly IPasswordHasher<Volo.Abp.Identity.IdentityUser> _passwordHasher;

    public AdministrationServiceInitialDataSeeder(
        ITenantRepository tenantRepository,
        ITenantManager tenantManager,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager,
        ICurrentTenant currentTenant,
        IPasswordHasher<Volo.Abp.Identity.IdentityUser> passwordHasher)
    {
        _tenantRepository = tenantRepository;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _permissionManager = permissionManager;
        _permissionDefinitionManager = permissionDefinitionManager;
        _currentTenant = currentTenant;
        _passwordHasher = passwordHasher;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        // 1. Ensure Tenant exists (Runs regardless of CurrentTenant)
        var targetTenant = await _tenantRepository.FindByNameAsync("LTCINEMA");
        if (targetTenant == null)
        {
            targetTenant = await _tenantManager.CreateAsync("LTCINEMA");
            await _tenantRepository.InsertAsync(targetTenant, autoSave: true);
        }

        // 2. Run the Role/User setup explicitly inside the LTCINEMA tenant context
        if (context.TenantId == targetTenant.Id)
        {
            await SeedTenantAdminAsync();
        }
        else if (context.TenantId == null)
        {
            // If running from Host context during migrations, switch to LTCINEMA and execute
            using (_currentTenant.Change(targetTenant.Id))
            {
                await SeedTenantAdminAsync();
            }
        }
    }

    private async Task SeedTenantAdminAsync()
    {
        // Role Setup
        var role = await _roleManager.FindByNameAsync("Admin");
        if (role == null)
        {
            role = new Volo.Abp.Identity.IdentityRole(Guid.NewGuid(), "Admin", _currentTenant.Id)
            {
                IsStatic = true,
                IsPublic = true
            };
            await _roleManager.CreateAsync(role);
        }

        // Grant ALL available permissions dynamically to 'Admin'
        var multiTenancySide = _currentTenant.Id.HasValue ? MultiTenancySides.Tenant : MultiTenancySides.Host;
        var permissions = await _permissionDefinitionManager.GetPermissionsAsync();
        
        foreach (var permission in permissions.Where(p => p.MultiTenancySide.HasFlag(multiTenancySide)))
        {
            try
            {
                await _permissionManager.SetForRoleAsync("Admin", permission.Name, true);
            }
            catch (ApplicationException)
            {
                // Some permission definitions are incompatible with role grants or currently disabled.
            }
        }

        // User Setup
        var user = await _userManager.FindByNameAsync("ADMIN");
        if (user == null)
        {
            user = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "ADMIN", "admin@ltcinema.com", _currentTenant.Id)
            {
                Name = "System",
                Surname = "Admin"
            };
            
            var result = await _userManager.CreateAsync(user, "Long2005");
            
            if (result.Succeeded)
            {
                await _userManager.AddDefaultRolesAsync(user);
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                throw new Exception("Error creating ADMIN: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
