using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Identity;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.Employee.Dtos.Output;
using LTC.AdministrationService.Entities;
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
        IRepository<Cinema, Guid> cinemaRepository,
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

        private static bool RequiresCinema(string? role)
        {
            var normalized = NormalizeRole(role);
            return string.Equals(normalized, "Manager", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Staff", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<Cinema?> GetCinemaAsync(Guid? cinemaId)
        {
            if (!cinemaId.HasValue)
            {
                return null;
            }

            return await cinemaRepository.FindAsync(cinemaId.Value);
        }

        private async Task EnsureCinemaRequirementAsync(string? role, Guid? cinemaId)
        {
            if (!RequiresCinema(role))
            {
                return;
            }

            if (!cinemaId.HasValue)
            {
                throw new UserFriendlyException("Cinema is required for manager/staff employees.");
            }

            var cinema = await GetCinemaAsync(cinemaId);
            if (cinema == null)
            {
                throw new UserFriendlyException("Cinema not found.");
            }
        }

        private async Task SyncManagerCinemaOwnershipAsync(Guid userId, string? role, Guid? cinemaId)
        {
            var cinemaQueryable = await cinemaRepository.GetQueryableAsync();

            if (!IsManagerRole(role))
            {
                var managedCinemas = await cinemaQueryable
                    .Where(x => x.ManagerUserId == userId)
                    .ToListAsync();

                if (managedCinemas.Count == 0)
                {
                    return;
                }

                foreach (var managedCinema in managedCinemas)
                {
                    managedCinema.ManagerUserId = null;
                    await cinemaRepository.UpdateAsync(managedCinema);
                }

                return;
            }

            if (!cinemaId.HasValue)
            {
                throw new UserFriendlyException("Manager must be assigned to one cinema.");
            }

            var targetCinema = await cinemaRepository.FindAsync(cinemaId.Value)
                ?? throw new UserFriendlyException("Cinema not found.");

            if (targetCinema.ManagerUserId.HasValue && targetCinema.ManagerUserId != userId)
            {
                throw new UserFriendlyException("This cinema already has a manager.");
            }

            var existingOwnership = await cinemaQueryable
                .Where(x => x.ManagerUserId == userId && x.Id != cinemaId.Value)
                .ToListAsync();

            foreach (var ownedCinema in existingOwnership)
            {
                ownedCinema.ManagerUserId = null;
                await cinemaRepository.UpdateAsync(ownedCinema);
            }

            targetCinema.ManagerUserId = userId;
            await cinemaRepository.UpdateAsync(targetCinema);
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
        public async Task<PagedResultEmployeeOutputDto> GetEmployeeListAsync(GetListEmployeeInputDto input)
        {
            var keyword = input.Keyword?.Trim();

            var employeesQueryable = await employeeRepository.GetQueryableAsync();

            var employeeRows = await employeesQueryable
                .Where(employee =>
                    (input.CinemaId == null || employee.CinemaId == input.CinemaId))
                .Select(employee => new
                {
                    employee.Id,
                    employee.UserId,
                    employee.CinemaId,
                    employee.PhoneNumber,
                    employee.HireDate
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
            var userProfileLookup = await identityUsersQueryable
                .Where(user => userIds.Contains(user.Id))
                .Select(user => new { user.Id, user.Name, user.Email, user.UserName, user.PhoneNumber })
                .ToDictionaryAsync(
                    user => user.Id,
                    user => new { user.Name, user.Email, user.UserName, user.PhoneNumber });
            var roleLookup = await BuildRoleLookupAsync(userIds);
            var cinemaIds = employeeRows
                .Select(x => x.CinemaId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();
            var cinemaLookup = await (await cinemaRepository.GetQueryableAsync())
                .Where(c => cinemaIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            var mappedEmployees = employeeRows
                .Select(row =>
                {
                    var profile = row.UserId.HasValue && userProfileLookup.TryGetValue(row.UserId.Value, out var profileValue)
                        ? profileValue
                        : null;
                    var cinemaName = row.CinemaId.HasValue && cinemaLookup.TryGetValue(row.CinemaId.Value, out var cinemaNameValue)
                        ? cinemaNameValue
                        : null;
                    var role = row.UserId.HasValue && roleLookup.TryGetValue(row.UserId.Value, out var roleValue) ? roleValue : "Staff";
                    var isActive = row.UserId.HasValue
                        && userActiveLookup.TryGetValue(row.UserId.Value, out var isActiveValue)
                        && isActiveValue;

                    return new EmployeeOutputDto
                    {
                        Id = row.Id,
                        UserId = row.UserId,
                        Name = profile?.Name ?? string.Empty,
                        Email = profile?.Email ?? string.Empty,
                        Code = profile?.UserName ?? string.Empty,
                        PhoneNumber = row.PhoneNumber ?? profile?.PhoneNumber,
                        CinemaId = row.CinemaId,
                        CinemaName = cinemaName,
                        OrganizationUnitId = row.CinemaId,
                        OrganizationUnitName = cinemaName,
                        HireDate = row.HireDate,
                        Role = role,
                        IsActive = isActive
                    };
                })
                .ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                mappedEmployees = mappedEmployees
                    .Where(e =>
                        (e.Name?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                        || (e.Email?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                        || (e.Code?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

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
        public async Task<EmployeeOutputDto> CreateEmployeeAsync(CreateEmployeeInputDto input)
        {
            using (var uow = unitOfWorkManager.Begin())
            {
                await EnsureCinemaRequirementAsync(input.Role, input.CinemaId);

                // tạo user
                var userId = await identityUserAppService.CreateIdentityUserAsync(new CreateUserInputDto
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
                    CinemaId = input.CinemaId,
                    HireDate = input.HireDate,
                    PhoneNumber = input.PhoneNumber,
                    Position = NormalizeRole(input.Role),
                    Status = "Active",
                    CreatedByUserId = CurrentUser.Id,
                    UserId = userId,
                };

                await employeeRepository.InsertAsync(employee);
                await SyncManagerCinemaOwnershipAsync(userId, input.Role, input.CinemaId);

                await uow.CompleteAsync();
                var createdEmployee = await GetEmployeeAsync(employee.Id);
                return createdEmployee ?? throw new UserFriendlyException("Failed to load created employee.");
            }
        }

        public async Task<EmployeeOutputDto?> GetEmployeeAsync(Guid id)
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
            var cinemaName = employee.CinemaId.HasValue
                ? (await cinemaRepository.FindAsync(employee.CinemaId.Value))?.Name
                : null;

            return new EmployeeOutputDto
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Name = user?.Name ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Code = user?.UserName ?? string.Empty,
                PhoneNumber = employee.PhoneNumber ?? user?.PhoneNumber,
                CinemaId = employee.CinemaId,
                CinemaName = cinemaName,
                OrganizationUnitId = employee.CinemaId,
                OrganizationUnitName = cinemaName,
                HireDate = employee.HireDate,
                Role = role,
                IsActive = user?.IsActive ?? false
            };
        }

        public async Task<EmployeeOutputDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeInputDto input)
        {
            using var uow = unitOfWorkManager.Begin();

            var employee = await employeeRepository.GetAsync(id);
            if (!employee.UserId.HasValue)
            {
                throw new UserFriendlyException(L["UserNotFound"]);
            }

            var user = await identityUserRepository.GetAsync(employee.UserId.Value);
            var currentRoles = await identityUserManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault() ?? "Staff";
            var resolvedRole = string.IsNullOrWhiteSpace(input.Role) ? currentRole : NormalizeRole(input.Role);
            var roleProvided = !string.IsNullOrWhiteSpace(input.Role);
            var cinemaProvided = input.CinemaId.HasValue;
            var shouldSyncManagerOwnership = roleProvided || cinemaProvided;

            // For partial updates that don't include role/cinema, preserve existing assignment.
            // This avoids false manager-duplication checks when editing unrelated fields.
            var resolvedCinemaId = input.CinemaId;
            if (!input.CinemaId.HasValue && RequiresCinema(resolvedRole))
            {
                resolvedCinemaId = employee.CinemaId;
            }

            await EnsureCinemaRequirementAsync(resolvedRole, resolvedCinemaId);

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

            if (!string.Equals(NormalizeRole(currentRole), resolvedRole, StringComparison.OrdinalIgnoreCase))
            {
                var roleResult = await identityUserManager.SetRolesAsync(user, [resolvedRole]);
                if (!roleResult.Succeeded)
                {
                    throw new UserFriendlyException(string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            employee.CinemaId = resolvedCinemaId;
            employee.HireDate = input.HireDate;
            employee.PhoneNumber = input.PhoneNumber;
            employee.Position = resolvedRole;
            employee.Status = input.IsActive ? "Active" : "Inactive";
            await employeeRepository.UpdateAsync(employee);
            if (shouldSyncManagerOwnership)
            {
                await SyncManagerCinemaOwnershipAsync(employee.UserId.Value, resolvedRole, resolvedCinemaId);
            }

            await uow.CompleteAsync();
            var updatedEmployee = await GetEmployeeAsync(id);
            return updatedEmployee ?? throw new UserFriendlyException("Failed to load updated employee.");
        }

        public async Task DeleteEmployeeAsync(Guid id)
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
