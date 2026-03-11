using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;

namespace LTC.AdministrationService.Permission
{
    public class PermissionAppService : AdministrationServiceAppService
    {
        protected PermissionManagementOptions Options { get; }
        protected IPermissionManager PermissionManager { get; }
        protected IPermissionChecker PermissionChecker { get; }
        protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected ISimpleStateCheckerManager<PermissionDefinition> SimpleStateCheckerManager { get; }

        public virtual async Task<GetPermissionListResultDto> GetAsync()
        {
            var result = new GetPermissionListResultDto
            {
                Groups = new List<PermissionGroupDto>()
            };

            foreach (var group in await PermissionDefinitionManager.GetGroupsAsync())
            {
                var groupDto = CreatePermissionGroupDto(group);

                var neededCheckPermissions = new List<PermissionDefinition>();

                foreach (var permission in group.GetPermissionsWithChildren().Where(x => x.IsEnabled))
                {
                    if (await SimpleStateCheckerManager.IsEnabledAsync(permission))
                    {
                        neededCheckPermissions.Add(permission);
                    }
                }

                if (!neededCheckPermissions.Any())
                {
                    continue;
                }

                var grantInfoDtos = neededCheckPermissions
                    .Select(CreatePermissionGrantInfoDto)
                    .ToList();

                var multipleGrantInfo = await PermissionManager.GetAsync(neededCheckPermissions.Select(x => x.Name).ToArray(), null, null);

                foreach (var grantInfo in multipleGrantInfo.Result)
                {
                    var grantInfoDto = grantInfoDtos.First(x => x.Name == grantInfo.Name);

                    grantInfoDto.IsGranted = grantInfo.IsGranted;

                    foreach (var provider in grantInfo.Providers)
                    {
                        grantInfoDto.GrantedProviders.Add(new ProviderInfoDto
                        {
                            ProviderName = provider.Name,
                            ProviderKey = provider.Key,
                        });
                    }

                    groupDto.Permissions.Add(grantInfoDto);
                }

                if (groupDto.Permissions.Any())
                {
                    result.Groups.Add(groupDto);
                }
            }

            return result;
        }

        public async Task<bool> IsGrantedAsync([NotNull] string permissionName)
        {
            return await PermissionChecker.IsGrantedAsync(permissionName);
        }

        public async Task<bool> IsGrantedMultiplePermissionsAsync(string[] permissionNames)
        {
            if (permissionNames.Length == 1)
            {
                return await PermissionChecker.IsGrantedAsync(permissionNames.First());
            }

            var multiplePermissionGrantResult = await PermissionChecker.IsGrantedAsync(permissionNames);
            return (multiplePermissionGrantResult.AllGranted || !multiplePermissionGrantResult.AllProhibited);
        }

        private PermissionGrantInfoDto CreatePermissionGrantInfoDto(PermissionDefinition permission)
        {
            return new PermissionGrantInfoDto
            {
                Name = permission.Name,
                DisplayName = permission.DisplayName?.Localize(StringLocalizerFactory),
                ParentName = permission.Parent?.Name,
                AllowedProviders = permission.Providers,
                GrantedProviders = new List<ProviderInfoDto>()
            };
        }

        private PermissionGroupDto CreatePermissionGroupDto(PermissionGroupDefinition group)
        {
            var localizableDisplayName = group.DisplayName as LocalizableString;

            return new PermissionGroupDto
            {
                Name = group.Name,
                DisplayName = group.DisplayName?.Localize(StringLocalizerFactory),
                DisplayNameKey = localizableDisplayName?.Name,
                DisplayNameResource = localizableDisplayName?.ResourceType != null
                    ? LocalizationResourceNameAttribute.GetName(localizableDisplayName.ResourceType)
                    : null,
                Permissions = new List<PermissionGrantInfoDto>()
            };
        }

        protected virtual async Task CheckProviderPolicy(string providerName)
        {
            var policyName = Options.ProviderPolicies.GetOrDefault(providerName);
            if (policyName.IsNullOrEmpty())
            {
                throw new AbpException($"No policy defined to get/set permissions for the provider '{providerName}'. Use {nameof(PermissionManagementOptions)} to map the policy.");
            }

            await AuthorizationService.CheckAsync(policyName);
        }
    }
}
