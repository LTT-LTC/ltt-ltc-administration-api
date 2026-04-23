using System;
using System.Linq;
using System.Threading.Tasks;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Employee;
using LTC.AdministrationService.Employee.Dtos.Input;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp;
using Xunit;
using EmployeeEntity = LTC.AdministrationService.Entities.Employee;

namespace LTC.AdministrationService.Employee;

public class EmployeeRbacIntegrationTests : AdministrationServiceApplicationTestBase<AdministrationServiceApplicationTestModule>
{
    private readonly IEmployeeAppService _employeeAppService;
    private readonly IRepository<EmployeeEntity, Guid> _employeeRepository;
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IdentityRoleManager _identityRoleManager;
    private readonly IdentityUserManager _identityUserManager;

    public EmployeeRbacIntegrationTests()
    {
        _employeeAppService = GetRequiredService<IEmployeeAppService>();
        _employeeRepository = GetRequiredService<IRepository<EmployeeEntity, Guid>>();
        _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
        _identityRoleManager = GetRequiredService<IdentityRoleManager>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
    }

    [Fact]
    public async Task Create_Should_Create_User_Employee_And_Assign_Role()
    {
        await EnsureRoleExistsAsync("Staff");

        var employeeId = await _employeeAppService.CreateAsync(new CreateEmployeeInputDto
        {
            Name = "Test Staff",
            Email = $"staff-{Guid.NewGuid():N}@example.com",
            Code = $"EMP-{Guid.NewGuid():N}",
            PhoneNumber = "0912345678",
            Role = "Staff",
            JoinedDate = DateTime.UtcNow.Date,
            DateOfBirth = DateTime.UtcNow.Date.AddYears(-20)
        });

        var employee = await _employeeRepository.GetAsync(employeeId);
        employee.UserId.ShouldNotBeNull();

        var user = await _identityUserRepository.GetAsync(employee.UserId!.Value);
        var roles = await _identityUserManager.GetRolesAsync(user);

        roles.ShouldContain("Staff");
    }

    [Fact]
    public async Task Update_Should_Sync_Identity_And_Role()
    {
        await EnsureRoleExistsAsync("Staff");
        await EnsureRoleExistsAsync("Manager");

        var employeeId = await _employeeAppService.CreateAsync(new CreateEmployeeInputDto
        {
            Name = "Before Update",
            Email = $"before-{Guid.NewGuid():N}@example.com",
            Code = $"EMP-{Guid.NewGuid():N}",
            PhoneNumber = "0912345678",
            Role = "Staff",
            JoinedDate = DateTime.UtcNow.Date,
            DateOfBirth = DateTime.UtcNow.Date.AddYears(-20)
        });

        var updateResult = await _employeeAppService.UpdateAsync(employeeId, new UpdateEmployeeInputDto
        {
            Name = "After Update",
            Email = $"after-{Guid.NewGuid():N}@example.com",
            Code = $"EMP-{Guid.NewGuid():N}",
            PhoneNumber = "0987654321",
            Role = "Manager",
            IsActive = false
        });

        updateResult.ShouldBeTrue();

        var employee = await _employeeRepository.GetAsync(employeeId);
        var user = await _identityUserRepository.GetAsync(employee.UserId!.Value);
        var roles = await _identityUserManager.GetRolesAsync(user);

        employee.Name.ShouldBe("After Update");
        user.IsActive.ShouldBeFalse();
        roles.Count.ShouldBe(1);
        roles.First().ShouldBe("Manager");
    }

    [Fact]
    public async Task Reconcile_Should_Report_Missing_Roles()
    {
        await EnsureRoleExistsAsync("Staff");

        var employeeId = await _employeeAppService.CreateAsync(new CreateEmployeeInputDto
        {
            Name = "Reconcile Test",
            Email = $"reconcile-{Guid.NewGuid():N}@example.com",
            Code = $"EMP-{Guid.NewGuid():N}",
            PhoneNumber = "0912345678",
            Role = "Staff",
            JoinedDate = DateTime.UtcNow.Date,
            DateOfBirth = DateTime.UtcNow.Date.AddYears(-20)
        });

        var employee = await _employeeRepository.GetAsync(employeeId);
        var user = await _identityUserRepository.GetAsync(employee.UserId!.Value);
        await _identityUserManager.SetRolesAsync(user, []);

        var result = await _employeeAppService.ReconcileIdentityLinksAsync();
        result.EmployeeIdsWithMissingRoles.ShouldContain(employeeId);
    }

    [Fact]
    public async Task Create_Should_Throw_When_Cinema_Already_Has_Manager()
    {
        await EnsureRoleExistsAsync("Manager");
        var cinemaId = Guid.NewGuid();

        await _employeeAppService.CreateAsync(new CreateEmployeeInputDto
        {
            Name = "Manager One",
            Email = $"manager-one-{Guid.NewGuid():N}@example.com",
            Code = $"EMP-{Guid.NewGuid():N}",
            PhoneNumber = "0912345678",
            Role = "Manager",
            OrganizationUnitId = cinemaId
        });

        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _employeeAppService.CreateAsync(new CreateEmployeeInputDto
            {
                Name = "Manager Two",
                Email = $"manager-two-{Guid.NewGuid():N}@example.com",
                Code = $"EMP-{Guid.NewGuid():N}",
                PhoneNumber = "0912345678",
                Role = "Manager",
                OrganizationUnitId = cinemaId
            }));
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        var role = await _identityRoleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            return;
        }

        var createResult = await _identityRoleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), roleName));
        createResult.Succeeded.ShouldBeTrue();
    }
}
