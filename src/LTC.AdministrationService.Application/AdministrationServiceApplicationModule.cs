using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.DependencyInjection;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Options;
using Microsoft.Extensions.Configuration;
using LTC.AdministrationService.Admin.Cinemas;
using LTC.AdministrationService.Admin.Screens;
using LTC.AdministrationService.Admin.SeatTypes;
using LTC.AdministrationService.Admin.SeatMaps;
using LTC.AdministrationService.Cinemas;
using LTC.AdministrationService.Screens;
using LTC.AdministrationService.SeatTypes;
using LTC.AdministrationService.SeatMaps;
using LTC.AdministrationService.Showtimes;

namespace LTC.AdministrationService;

[DependsOn(
    typeof(AbpMapperlyModule),
    typeof(AdministrationServiceDomainModule),
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpIdentityAspNetCoreModule)
    )]
public class AdministrationServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<InternalApiOptions>(configuration.GetSection(InternalApiOptions.SectionName));
        Configure<SeatHoldOptions>(configuration.GetSection(SeatHoldOptions.SectionName));
        context.Services.AddTransient<ISeatHoldRealtimeNotifier, NullSeatHoldRealtimeNotifier>();

        // Keep explicit bindings for admin app services to avoid runtime activation issues
        // when namespace refactors make convention-based resolution brittle.
        context.Services.AddTransient<IAdminCinemaAppService, CinemaAppService>();
        context.Services.AddTransient<IAdminCinemaAmenityAppService, CinemaAmenityAppService>();
        context.Services.AddTransient<IAdminScreenAppService, ScreenAppService>();
        context.Services.AddTransient<IAdminSeatTypeAppService, SeatTypeAppService>();
        context.Services.AddTransient<IAdminSeatMapAppService, SeatMapAppService>();

        context.Services.AddSingleton<ShowtimeSeatHoldStore>();
        context.Services.AddTransient<IShowtimeSeatHoldAppService, ShowtimeSeatHoldAppService>();
        context.Services.AddTransient<IShowtimeSeatLayoutMergeAppService, ShowtimeSeatLayoutMergeAppService>();
    }
}
