using LTC.AdministrationService.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

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
