using LTC.AdministrationService.Localization;
using Localization.Resources.AbpUi;
using LTC.AdministrationService;
using LTC.AdministrationService.Localization;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Realtime;

namespace LTC.AdministrationService;

[DependsOn(
   typeof(AdministrationServiceApplicationModule),
   typeof(AdministrationServiceApplicationContractsModule),
   typeof(AbpPermissionManagementHttpApiModule),
   typeof(AbpSettingManagementHttpApiModule),
   typeof(AbpAccountHttpApiModule),
   typeof(AbpIdentityHttpApiModule),
   typeof(AbpTenantManagementHttpApiModule),
   typeof(AbpFeatureManagementHttpApiModule)
   )]
public class AdministrationServiceHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSignalR();
        context.Services.Replace(ServiceDescriptor.Transient<ISeatHoldRealtimeNotifier, SeatHoldSignalRNotifier>());
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AdministrationServiceResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
