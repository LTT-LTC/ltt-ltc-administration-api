using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Identity;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Employee.Dtos.Output;
using LTC.Shared.CrossCuttingConcerns.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using LTC.AdministrationService.Identity.Dtos.Input;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace LTC.AdministrationService
{
    public class EmployeeAppService(
        IRepository<Entities.Employee, Guid> employeeRepository,
        IRepository<IdentityUser, Guid> identityUserRepository,
        IUnitOfWorkManager unitOfWorkManager,
        LTC.AdministrationService.Identity.IIdentityUserAppService identityUserAppService,
        IdentityUserManager identityUserManager
        ) : AdministrationServiceAppService, IEmployeeAppService
    {
        private static string NormalizeRole(string? role) =>
            string.IsNullOrWhiteSpace(role) ? "Staff" : role.Trim();

        private static bool IsManagerRole(string? role) =>
            string.Equals(NormalizeRole(role), "Manager", StringComparison.OrdinalIgnoreCase);

        private async Task EnsureSingleManagerPerCinemaAsync(Guid? cinemaId, Guid? excludeEmployeeId = null)
        {
            if (!cinemaId.HasValue)
            {
                throw new UserFriendlyException("Cinema is required when assigning Manager role.");
            }

            var employeesQueryable = await employeeRepository.GetQueryableAsync();
            var managerCandidates = await employeesQueryable
                .Where(x =>
                    x.OrganizationUnitId == cinemaId
                    && x.UserId.HasValue
                    && (!excludeEmployeeId.HasValue || x.Id != excludeEmployeeId.Value))
                .Select(x => new { x.Id, x.UserId })
                .ToListAsync();

            foreach (var candidate in managerCandidates)
            {
                var user = await identityUserRepository.FindAsync(candidate.UserId!.Value);
                if (user == null || !user.IsActive)
                {
                    continue;
                }

                var roles = await identityUserManager.GetRolesAsync(user);
                if (roles.Any(r => string.Equals(r, "Manager", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new UserFriendlyException("Each cinema can only have one active manager.");
                }
            }
        }

        private async Task<Dictionary<Guid, string>> BuildRoleLookupAsync(List<Guid> userIds)
        {
            var roleLookup = new Dictionary<Guid, string>();
            foreach (var userId in userIds)
            {
                var user = await identityUserRepository.FindAsync(userId);
                if (user == null)
                {
                    roleLookup[userId] = "Staff";
                    continue;
                }

                var roles = await identityUserManager.GetRolesAsync(user);
                roleLookup[userId] = roles.FirstOrDefault() ?? "Staff";
            }

            return roleLookup;
        }

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

            var identityUsersQueryable = await identityUserRepository.GetQueryableAsync();
            var userActiveLookup = await identityUsersQueryable
                .Where(user => userIds.Contains(user.Id))
                .Select(user => new { user.Id, user.IsActive })
                .ToDictionaryAsync(user => user.Id, user => user.IsActive);
            var roleLookup = await BuildRoleLookupAsync(userIds);

            var mappedEmployees = employeeRows
                .Select(row => new EmployeeOutputDto
                {
                    Id = row.Id,
                    UserId = row.UserId,
                    Name = row.Name,
                    Email = row.Email,
                    Code = row.Code,
                    PositionId = row.PositionId,
                    OrganizationUnitId = row.OrganizationUnitId,
                    Role = row.UserId.HasValue && roleLookup.TryGetValue(row.UserId.Value, out var role) ? role : "Staff",
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
            using (var uow = unitOfWorkManager.Begin())
            {
                if (IsManagerRole(input.Role))
                {
                    await EnsureSingleManagerPerCinemaAsync(input.OrganizationUnitId);
                }

                // tạo user
                var userId = await identityUserAppService.CreateAsync(new CreateUserInputDto
                {
                    UserName = input.Email,
                    Name = input.Name,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumber,
                    Surname = input.Name,
                    IsActive = true,
                    Roles = [NormalizeRole(input.Role)]
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

        public async Task<EmployeeOutputDto?> GetAsync(Guid id)
        {
            var employee = await employeeRepository.FindAsync(id);
            if (employee == null)
            {
                return null;
            }

            IdentityUser? user = null;
            string role = "Staff";
            if (employee.UserId.HasValue)
            {
                user = await identityUserRepository.FindAsync(employee.UserId.Value);
                if (user != null)
                {
                    var roles = await identityUserManager.GetRolesAsync(user);
                    role = roles.FirstOrDefault() ?? role;
                }
            }

            return new EmployeeOutputDto
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Name = employee.Name ?? string.Empty,
                Email = employee.Email ?? string.Empty,
                Code = employee.Code ?? string.Empty,
                PositionId = employee.PositionId,
                OrganizationUnitId = employee.OrganizationUnitId,
                Role = role,
                IsActive = user?.IsActive ?? false
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateEmployeeInputDto input)
        {
            using var uow = unitOfWorkManager.Begin();

            var employee = await employeeRepository.GetAsync(id);
            if (!employee.UserId.HasValue)
            {
                throw new UserFriendlyException(L["UserNotFound"]);
            }

            if (IsManagerRole(input.Role))
            {
                await EnsureSingleManagerPerCinemaAsync(input.OrganizationUnitId, id);
            }

            var user = await identityUserRepository.GetAsync(employee.UserId.Value);
            user.Name = input.Name;
            user.Surname = input.Name;
            var setUserNameResult = await identityUserManager.SetUserNameAsync(user, input.Email);
            if (!setUserNameResult.Succeeded)
            {
                throw new UserFriendlyException(string.Join("; ", setUserNameResult.Errors.Select(e => e.Description)));
            }

            var setEmailResult = await identityUserManager.SetEmailAsync(user, input.Email);
            if (!setEmailResult.Succeeded)
            {
                throw new UserFriendlyException(string.Join("; ", setEmailResult.Errors.Select(e => e.Description)));
            }

            user.SetPhoneNumber(input.PhoneNumber, true);
            user.SetIsActive(input.IsActive);

            var updateResult = await identityUserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new UserFriendlyException(string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            var roleResult = await identityUserManager.SetRolesAsync(user, [NormalizeRole(input.Role)]);
            if (!roleResult.Succeeded)
            {
                throw new UserFriendlyException(string.Join("; ", roleResult.Errors.Select(e => e.Description)));
            }

            employee.Name = input.Name;
            employee.Email = input.Email;
            employee.OtherEmail = input.OtherEmail;
            employee.PhoneNumber = input.PhoneNumber;
            employee.Code = input.Code;
            employee.OrganizationUnitId = input.OrganizationUnitId;
            employee.PositionId = input.PositionId;
            employee.JoinedDate = input.JoinedDate;
            employee.DateOfBirth = input.DateOfBirth;
            await employeeRepository.UpdateAsync(employee);

            await uow.CompleteAsync();
            return true;
        }

        public async Task DeleteAsync(Guid id)
        {
            using var uow = unitOfWorkManager.Begin();
            var employee = await employeeRepository.GetAsync(id);

            if (employee.UserId.HasValue)
            {
                await identityUserRepository.DeleteAsync(employee.UserId.Value);
            }

            await employeeRepository.DeleteAsync(id);
            await uow.CompleteAsync();
        }

        public async Task<EmployeeIdentityReconciliationResultDto> ReconcileIdentityLinksAsync()
        {
            var result = new EmployeeIdentityReconciliationResultDto();
            var employees = await employeeRepository.GetListAsync();
            result.TotalEmployees = employees.Count;

            foreach (var employee in employees)
            {
                if (!employee.UserId.HasValue)
                {
                    result.MissingUserLinks++;
                    result.EmployeeIdsMissingLinks.Add(employee.Id);
                    continue;
                }

                var user = await identityUserRepository.FindAsync(employee.UserId.Value);
                if (user == null)
                {
                    result.MissingUsers++;
                    result.EmployeeIdsWithMissingUsers.Add(employee.Id);
                    continue;
                }

                var roles = await identityUserManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    result.MissingRoles++;
                    result.EmployeeIdsWithMissingRoles.Add(employee.Id);
                }
            }

            return result;
        }
    }
}
