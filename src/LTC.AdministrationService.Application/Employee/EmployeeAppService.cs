using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Identity;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Employee.Dtos.Output;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using LTC.AdministrationService.Identity.Dtos.Input;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace LTC.AdministrationService
{
    public class EmployeeAppService(
        IRepository<Entities.Employee, Guid> employeeRepository,
        IRepository<IdentityUser, Guid> _identityUserRepository,
        IUnitOfWorkManager _unitOfWorkManager,
        IIdentityUserAppService identityUserAppService
        ) : AdministrationServiceAppService, IEmployeeAppService
    {
        /// <summary>
        /// Get List Employee
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultEmployeeOutputDto> GetListAsync(GetListEmployeeInputDto input)
        {
            var keyword = input.Keyword?.Trim();

            var employeesQueryable = await employeeRepository.GetQueryableAsync();

            var employeeRows = await employeesQueryable
                .Where(employee =>
                    (string.IsNullOrEmpty(keyword) || employee.Name.Contains(keyword) || employee.Code.Contains(keyword))
                    && (input.CinemaId == null || employee.CinemaId == input.CinemaId || employee.OrganizationUnitId == input.CinemaId))
                .Select(employee => new
                {
                    employee.Id,
                    employee.UserId,
                    employee.Name,
                    employee.Email,
                    employee.Code,
                    employee.PositionId,
                    employee.OrganizationUnitId
                })
                .ToListAsync();

            var userIds = employeeRows
                .Select(x => x.UserId)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .Distinct()
                .ToList();

            var identityUsersQueryable = await _identityUserRepository.GetQueryableAsync();
            var userActiveLookup = await identityUsersQueryable
                .Where(user => userIds.Contains(user.Id))
                .Select(user => new { user.Id, user.IsActive })
                .ToDictionaryAsync(user => user.Id, user => user.IsActive);

            var mappedEmployees = employeeRows
                .Select(row => new EmployeeOutputDto
                {
                    Id = row.Id,
                    Name = row.Name,
                    Email = row.Email,
                    Code = row.Code,
                    PositionId = row.PositionId,
                    OrganizationUnitId = row.OrganizationUnitId,
                    IsActive = row.UserId.HasValue
                        && userActiveLookup.TryGetValue(row.UserId.Value, out var isActive)
                        && isActive
                })
                .ToList();

            var totalActiveEmployees = mappedEmployees.Count(e => e.IsActive);
            var totalDeactiveEmployees = mappedEmployees.Count - totalActiveEmployees;

            if (input.IsActive.HasValue)
            {
                mappedEmployees = mappedEmployees
                    .Where(e => e.IsActive == input.IsActive.Value)
                    .ToList();
            }

            var totalCount = mappedEmployees.Count;

            var employees = mappedEmployees
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToList();

            var result = new PagedResultEmployeeOutputDto(totalCount, employees);
            result.ExtendData = new Dictionary<string, object>
            {
                { "totalActiveEmployees", totalActiveEmployees },
                { "totalDeactiveEmployees", totalDeactiveEmployees }
            };

            return result;
        }

        /// <summary>
        /// tạo nhân sự
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Guid> CreateAsync(CreateEmployeeInputDto input)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // tạo user
                var userId = await identityUserAppService.CreateAsync(new CreateUserInputDto
                {
                    UserName = input.Email,
                    Name = input.Name,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber,
                    Surname = input.Name,
                    IsActive = true
                });

                var employee = new Entities.Employee
                {
                    Name = input.Name,
                    Code = input.Code,
                    Email = input.Email,
                    OtherEmail = input.OtherEmail,
                    PhoneNumber = input.PhoneNumber,
                    OrganizationUnitId = input.OrganizationUnitId,
                    PositionId = input.PositionId,
                    AvatarFileId = null,
                    JoinedDate = input.JoinedDate,
                    DateOfBirth = input.DateOfBirth,
                    UserId = userId,
                    IsFirstLogin = true
                };

                await employeeRepository.InsertAsync(employee);

                await uow.CompleteAsync();
                return employee.Id;
            }
        }
    }
}
