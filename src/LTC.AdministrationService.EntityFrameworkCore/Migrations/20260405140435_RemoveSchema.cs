using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Showtimes",
                schema: "LTC",
                newName: "Showtimes");

            migrationBuilder.RenameTable(
                name: "SeatTypes",
                schema: "LTC",
                newName: "SeatTypes");

            migrationBuilder.RenameTable(
                name: "Screens",
                schema: "LTC",
                newName: "Screens");

            migrationBuilder.RenameTable(
                name: "RevenueSnapshots",
                schema: "LTC",
                newName: "RevenueSnapshots");

            migrationBuilder.RenameTable(
                name: "PricingRules",
                schema: "LTC",
                newName: "PricingRules");

            migrationBuilder.RenameTable(
                name: "MediaFiles",
                schema: "LTC",
                newName: "MediaFiles");

            migrationBuilder.RenameTable(
                name: "GiftCodes",
                schema: "LTC",
                newName: "GiftCodes");

            migrationBuilder.RenameTable(
                name: "Employees",
                schema: "LTC",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "Cinemas",
                schema: "LTC",
                newName: "Cinemas");

            migrationBuilder.RenameTable(
                name: "CinemaAmenityTypes",
                schema: "LTC",
                newName: "CinemaAmenityTypes");

            migrationBuilder.RenameTable(
                name: "CinemaAmenities",
                schema: "LTC",
                newName: "CinemaAmenities");

            migrationBuilder.RenameTable(
                name: "AbpUsers",
                schema: "LTC",
                newName: "AbpUsers");

            migrationBuilder.RenameTable(
                name: "AbpUserRoles",
                schema: "LTC",
                newName: "AbpUserRoles");

            migrationBuilder.RenameTable(
                name: "AbpUserPasswordHistories",
                schema: "LTC",
                newName: "AbpUserPasswordHistories");

            migrationBuilder.RenameTable(
                name: "AbpUserPasskeys",
                schema: "LTC",
                newName: "AbpUserPasskeys");

            migrationBuilder.RenameTable(
                name: "AbpUserOrganizationUnits",
                schema: "LTC",
                newName: "AbpUserOrganizationUnits");

            migrationBuilder.RenameTable(
                name: "AbpUserDelegations",
                schema: "LTC",
                newName: "AbpUserDelegations");

            migrationBuilder.RenameTable(
                name: "AbpTenants",
                schema: "LTC",
                newName: "AbpTenants");

            migrationBuilder.RenameTable(
                name: "AbpSettings",
                schema: "LTC",
                newName: "AbpSettings");

            migrationBuilder.RenameTable(
                name: "AbpSettingDefinitions",
                schema: "LTC",
                newName: "AbpSettingDefinitions");

            migrationBuilder.RenameTable(
                name: "AbpRoles",
                schema: "LTC",
                newName: "AbpRoles");

            migrationBuilder.RenameTable(
                name: "AbpResourcePermissionGrants",
                schema: "LTC",
                newName: "AbpResourcePermissionGrants");

            migrationBuilder.RenameTable(
                name: "AbpPermissions",
                schema: "LTC",
                newName: "AbpPermissions");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGroups",
                schema: "LTC",
                newName: "AbpPermissionGroups");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGrants",
                schema: "LTC",
                newName: "AbpPermissionGrants");

            migrationBuilder.RenameTable(
                name: "AbpOrganizationUnits",
                schema: "LTC",
                newName: "AbpOrganizationUnits");

            migrationBuilder.RenameTable(
                name: "AbpFeatureValues",
                schema: "LTC",
                newName: "AbpFeatureValues");

            migrationBuilder.RenameTable(
                name: "AbpFeatures",
                schema: "LTC",
                newName: "AbpFeatures");

            migrationBuilder.RenameTable(
                name: "AbpFeatureGroups",
                schema: "LTC",
                newName: "AbpFeatureGroups");

            migrationBuilder.RenameTable(
                name: "AbpClaimTypes",
                schema: "LTC",
                newName: "AbpClaimTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LTC");

            migrationBuilder.RenameTable(
                name: "Showtimes",
                newName: "Showtimes",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "SeatTypes",
                newName: "SeatTypes",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "Screens",
                newName: "Screens",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "RevenueSnapshots",
                newName: "RevenueSnapshots",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "PricingRules",
                newName: "PricingRules",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "MediaFiles",
                newName: "MediaFiles",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "GiftCodes",
                newName: "GiftCodes",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employees",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "Cinemas",
                newName: "Cinemas",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "CinemaAmenityTypes",
                newName: "CinemaAmenityTypes",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "CinemaAmenities",
                newName: "CinemaAmenities",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpUsers",
                newName: "AbpUsers",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpUserRoles",
                newName: "AbpUserRoles",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpUserPasswordHistories",
                newName: "AbpUserPasswordHistories",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpUserPasskeys",
                newName: "AbpUserPasskeys",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpUserOrganizationUnits",
                newName: "AbpUserOrganizationUnits",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpUserDelegations",
                newName: "AbpUserDelegations",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpTenants",
                newName: "AbpTenants",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpSettings",
                newName: "AbpSettings",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpSettingDefinitions",
                newName: "AbpSettingDefinitions",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpRoles",
                newName: "AbpRoles",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpResourcePermissionGrants",
                newName: "AbpResourcePermissionGrants",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpPermissions",
                newName: "AbpPermissions",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGroups",
                newName: "AbpPermissionGroups",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGrants",
                newName: "AbpPermissionGrants",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpOrganizationUnits",
                newName: "AbpOrganizationUnits",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpFeatureValues",
                newName: "AbpFeatureValues",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpFeatures",
                newName: "AbpFeatures",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpFeatureGroups",
                newName: "AbpFeatureGroups",
                newSchema: "LTC");

            migrationBuilder.RenameTable(
                name: "AbpClaimTypes",
                newName: "AbpClaimTypes",
                newSchema: "LTC");
        }
    }
}
