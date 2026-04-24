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
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Code { get; set; }
        public Guid? CinemaId { get; set; }
        public string? CinemaName { get; set; }
        public DateTime? HireDate { get; set; }
        // Backward-compatibility fields for FE pages still reading old names.
        public Guid? OrganizationUnitId { get; set; }
        public string? OrganizationUnitName { get; set; }
        public bool IsActive { get; set; }
        public string? Role { get; set; }
    }

    public class PagedResultEmployeeOutputDto(int totalCount, IReadOnlyList<EmployeeOutputDto> items)
    {
        public Dictionary<string, object> ExtendData { get; set; }
        public int TotalCount { get; set; } = totalCount;
        public IReadOnlyList<EmployeeOutputDto> Items { get; set; } = items;
    }

    public class EmployeeIdentityReconciliationResultDto
    {
        public int TotalEmployees { get; set; }
        public int MissingUserLinks { get; set; }
        public int MissingUsers { get; set; }
        public int MissingRoles { get; set; }
        public List<Guid> EmployeeIdsMissingLinks { get; set; } = [];
        public List<Guid> EmployeeIdsWithMissingUsers { get; set; } = [];
        public List<Guid> EmployeeIdsWithMissingRoles { get; set; } = [];
    }
}
