using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

namespace LTC.AdministrationService.Controllers;

        [Authorize(Roles = "Admin,Manager")]
public class HomeController : AbpController
{
    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }
}
