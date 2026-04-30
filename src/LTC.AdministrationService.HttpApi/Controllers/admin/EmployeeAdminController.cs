using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Controllers.Admin;

namespace LTC.AdministrationService.Controllers.Admin
{
    /// <summary>
    /// Admin-only Employee operations: create, update, delete.
    /// </summary>
    [Route(AdministrationServiceSettingNames.DefaultRoute + "/admin/employee")]
    public class EmployeeAdminController : AdminControllerBase
    {
        private readonly IEmployeeAppService _employeeAppService;

        public EmployeeAdminController(IEmployeeAppService employeeAppService)
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

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync([FromForm] CreateEmployeeInputDto input)
        {
            var result = await _employeeAppService.CreateEmployeeAsync(input);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeAsync(Guid id, [FromBody] UpdateEmployeeInputDto input)
        {
            var result = await _employeeAppService.UpdateEmployeeAsync(id, input);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            await _employeeAppService.DeleteEmployeeAsync(id);
            return Ok();
        }

        [HttpPost("reconcile")]
        public async Task<IActionResult> ReconcileIdentityLinksAsync()
        {
            var result = await _employeeAppService.ReconcileIdentityLinksAsync();
            return Ok(result);
        }

    }
}
