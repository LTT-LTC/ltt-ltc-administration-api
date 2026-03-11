using LTC.AdministrationService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AdministrationServiceController : AbpControllerBase
{
    protected AdministrationServiceController()
    {
        LocalizationResource = typeof(AdministrationServiceResource);
    }
}
