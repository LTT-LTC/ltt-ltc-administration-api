using LTC.AdministrationService.Localization;
using LTC.AdministrationService;
using LTC.AdministrationService.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace LTC.AdministrationService;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpTenantManagementDomainSharedModule),
    typeof(BlobStoringDatabaseDomainSharedModule)
    )]
public class AdministrationServiceDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AdministrationServiceGlobalFeatureConfigurator.Configure();
        AdministrationServiceModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AdministrationServiceDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<AdministrationServiceResource>("vi")
                //.Add<AdministrationServiceResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/AdministrationService");

            options.DefaultResourceType = typeof(AdministrationServiceResource);

            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("vi", "vi", "Vietnam"));
            //options.Languages.Add(new LanguageInfo("ar", "ar", "Arabic")); 
            //options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "Chinese (Simplified)")); 
            //options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "Chinese (Traditional)")); 
            //options.Languages.Add(new LanguageInfo("cs", "cs", "Czech")); 
            //options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (United Kingdom)")); 
            //options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish")); 
            //options.Languages.Add(new LanguageInfo("fr", "fr", "French")); 
            //options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "German (Germany)")); 
            //options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi ")); 
            //options.Languages.Add(new LanguageInfo("hu", "hu", "Hungarian")); 
            //options.Languages.Add(new LanguageInfo("is", "is", "Icelandic")); 
            //options.Languages.Add(new LanguageInfo("it", "it", "Italian")); 
            //options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Portuguese (Brazil)")); 
            //options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Romanian (Romania)")); 
            //options.Languages.Add(new LanguageInfo("ru", "ru", "Russian")); 
            //options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak")); 
            //options.Languages.Add(new LanguageInfo("es", "es", "Spanish")); 
            //options.Languages.Add(new LanguageInfo("sv", "sv", "Swedish")); 
            //options.Languages.Add(new LanguageInfo("tr", "tr", "Turkish")); 

        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("AdministrationService", typeof(AdministrationServiceResource));
        });
    }
}
