using System.Threading.Tasks;
using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.Shared.Hosting.Microservices.HttpApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers
{
    /// <summary>
    /// Public employee self-registration (no role prefix).
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/employee")]
    public class EmployeeRegistrationController(IEmployeeAppService employeeAppService) : AppControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync([FromForm] CreateEmployeeInputDto input)
            => Success(await employeeAppService.CreateAsync(input));
    }
}
