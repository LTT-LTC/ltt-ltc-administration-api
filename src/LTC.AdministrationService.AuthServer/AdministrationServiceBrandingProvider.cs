using Microsoft.Extensions.Localization;
using LTC.AdministrationService.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace LTC.AdministrationService;

[Dependency(ReplaceServices = true)]
public class AdministrationServiceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AdministrationServiceResource> _localizer;

    public AdministrationServiceBrandingProvider(IStringLocalizer<AdministrationServiceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
