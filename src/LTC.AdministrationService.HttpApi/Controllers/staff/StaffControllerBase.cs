using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace LTC.AdministrationService.Controllers.Staff
{
    /// <summary>
    /// Base controller for Staff operations.
    /// Staff have read-only access to specific resources.
    /// </summary>
    [RemoteService]
    [Area("staff")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public abstract class StaffControllerBase : AdministrationServiceController
    {
    }
}
