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
        Task<PagedResultEmployeeOutputDto> GetListAsync(GetListEmployeeInputDto input);
        Task<EmployeeOutputDto?> GetAsync(Guid id);
        Task<Guid> CreateAsync(CreateEmployeeInputDto input);
        Task<bool> UpdateAsync(Guid id, UpdateEmployeeInputDto input);
        Task DeleteAsync(Guid id);
        Task<EmployeeIdentityReconciliationResultDto> ReconcileIdentityLinksAsync();
    }
}
