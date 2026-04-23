using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Base controller for Admin-only operations.
    /// All methods in admin controllers require Admin role.
    /// </summary>
    [RemoteService]
    [Area("admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public abstract class AdminControllerBase : AdministrationServiceController
    {
    }
}
