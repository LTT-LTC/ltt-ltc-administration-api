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
        IRepository<IdentityUser, Guid> identityUserRepository,
        IUnitOfWorkManager unitOfWorkManager,
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
            var identityUsersQueryable = await identityUserRepository.GetQueryableAsync();

            // TODO: query theo phòng ban và vai trò
            var query = from employee in employeesQueryable
                        join identityUser in identityUsersQueryable on employee.UserId equals identityUser.Id
                        where string.IsNullOrEmpty(keyword) || employee.Name.Contains(keyword) || employee.Code.Contains(keyword)
                        select new EmployeeOutputDto
                        {
                            Id = employee.Id,
                            Name = employee.Name,
                            Email = employee.Email,
                            Code = employee.Code,
                            PositionId = employee.PositionId,
                            OrganizationUnitId = employee.OrganizationUnitId,
                            IsActive = identityUser.IsActive
                        };

            var totalCount = await query.CountAsync();
            var totalActiveEmployees = await query.CountAsync(e => e.IsActive);
            var totalDeactiveEmployees = totalCount - totalActiveEmployees;

            if (input.IsActive.HasValue)
            {
                totalCount = await query.CountAsync(e => e.IsActive == input.IsActive.Value);
                query = query.Where(e => e.IsActive == input.IsActive.Value);
            }

            var employees = await query
                .Skip((input.Page - 1) * input.Fetch)
                .Take(input.Fetch)
                .ToListAsync();

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
            using (var uow = unitOfWorkManager.Begin())
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
