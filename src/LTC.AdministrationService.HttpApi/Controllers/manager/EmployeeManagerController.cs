using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Controllers.Manager;

namespace LTC.AdministrationService.Controllers.Manager
{
    /// <summary>
    /// Manager Employee operations: read-only access to employee list and details.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/manager/employee")]
    public class EmployeeManagerController : ManagerControllerBase
    {
        private readonly IEmployeeAppService _employeeAppService;

        public EmployeeManagerController(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeListAsync([FromQuery] GetListEmployeeInputDto input)
        {
            var result = await _employeeAppService.GetEmployeeListAsync(input);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeAsync(Guid id)
        {
            var result = await _employeeAppService.GetEmployeeAsync(id);
            return Ok(result);
        }

    }
}
