using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace LTC.AdministrationService.Employee.Dtos.Output
{
    public class EmployeeOutputDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? OrganizationUnitId { get; set; }
        public bool IsActive { get; set; }
    }

    public class PagedResultEmployeeOutputDto(int totalCount, IReadOnlyList<EmployeeOutputDto> items)
    {
        public Dictionary<string, object> ExtendData { get; set; }
        public int TotalCount { get; set; } = totalCount;
        public IReadOnlyList<EmployeeOutputDto> Items { get; set; } = items;
    }
}
