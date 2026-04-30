using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Employee.Dtos.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace LTC.AdministrationService.Employee
{
    public interface IEmployeeAppService
    {
        Task<PagedResultEmployeeOutputDto> GetEmployeeListAsync(GetListEmployeeInputDto input);
        Task<EmployeeOutputDto?> GetEmployeeAsync(Guid id);
        Task<EmployeeOutputDto> CreateEmployeeAsync(CreateEmployeeInputDto input);
        Task<EmployeeOutputDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeInputDto input);
        Task DeleteEmployeeAsync(Guid id);
        Task<EmployeeIdentityReconciliationResultDto> ReconcileIdentityLinksAsync();
    }
}
