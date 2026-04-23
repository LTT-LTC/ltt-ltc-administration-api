using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace LTC.AdministrationService.Controllers.Pos
{
    /// <summary>
    /// Base controller for POS (point of sale) operations.
    /// </summary>
    [RemoteService]
    [Area("pos")]
    [ApiController]
    [Authorize(Roles = "POS")]
    public abstract class PosControllerBase : AdministrationServiceController
    {
    }
}
