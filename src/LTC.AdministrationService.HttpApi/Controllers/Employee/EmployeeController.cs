using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.Shared.Hosting.Microservices.HttpApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace LTC.AdministrationService.Controllers.Employee
{
    [Route($"{AdministrationServiceSettingNames.DefaultRoute}/employee")]
    public class EmployeeController(
        IEmployeeAppService employeeAppService
        ) : AppControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] GetListEmployeeInputDto input)
            => Success(await employeeAppService.GetListAsync(input));

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync([FromForm] CreateEmployeeInputDto input)
            => Success(await employeeAppService.CreateAsync(input));
    }
}
