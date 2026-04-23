using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Base controller for Manager operations.
    /// Managers have read access to most resources and limited write access per entity policy.
    /// </summary>
    [RemoteService]
    [Area("manager")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public abstract class ManagerControllerBase : AdministrationServiceController
    {
    }
}
