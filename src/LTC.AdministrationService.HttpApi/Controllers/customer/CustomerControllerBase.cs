using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace LTC.AdministrationService.Controllers.Customer
{
    [RemoteService]
    [Area("customer")]
    [ApiController]
    [Authorize]
    public abstract class CustomerControllerBase : AdministrationServiceController
    {
    }
}
