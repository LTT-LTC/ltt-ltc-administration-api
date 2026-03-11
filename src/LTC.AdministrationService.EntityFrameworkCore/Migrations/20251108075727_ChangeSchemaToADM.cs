using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSchemaToADM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ADM");

            migrationBuilder.RenameTable(
                name: "OpenIddictTokens",
                newName: "OpenIddictTokens",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "OpenIddictScopes",
                newName: "OpenIddictScopes",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "OpenIddictAuthorizations",
                newName: "OpenIddictAuthorizations",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "OpenIddictApplications",
                newName: "OpenIddictApplications",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUserTokens",
                newName: "AbpUserTokens",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUsers",
                newName: "AbpUsers",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUserRoles",
                newName: "AbpUserRoles",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUserOrganizationUnits",
                newName: "AbpUserOrganizationUnits",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUserLogins",
                newName: "AbpUserLogins",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUserDelegations",
                newName: "AbpUserDelegations",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpUserClaims",
                newName: "AbpUserClaims",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpTenants",
                newName: "AbpTenants",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpTenantConnectionStrings",
                newName: "AbpTenantConnectionStrings",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpSettings",
                newName: "AbpSettings",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpSettingDefinitions",
                newName: "AbpSettingDefinitions",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpSessions",
                newName: "AbpSessions",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpSecurityLogs",
                newName: "AbpSecurityLogs",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpRoles",
                newName: "AbpRoles",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpRoleClaims",
                newName: "AbpRoleClaims",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpPermissions",
                newName: "AbpPermissions",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGroups",
                newName: "AbpPermissionGroups",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGrants",
                newName: "AbpPermissionGrants",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpOrganizationUnits",
                newName: "AbpOrganizationUnits",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpOrganizationUnitRoles",
                newName: "AbpOrganizationUnitRoles",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpLinkUsers",
                newName: "AbpLinkUsers",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpFeatureValues",
                newName: "AbpFeatureValues",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpFeatures",
                newName: "AbpFeatures",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpFeatureGroups",
                newName: "AbpFeatureGroups",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpEntityPropertyChanges",
                newName: "AbpEntityPropertyChanges",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpEntityChanges",
                newName: "AbpEntityChanges",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpClaimTypes",
                newName: "AbpClaimTypes",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpBlobs",
                newName: "AbpBlobs",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpBlobContainers",
                newName: "AbpBlobContainers",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpBackgroundJobs",
                newName: "AbpBackgroundJobs",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpAuditLogs",
                newName: "AbpAuditLogs",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpAuditLogExcelFiles",
                newName: "AbpAuditLogExcelFiles",
                newSchema: "ADM");

            migrationBuilder.RenameTable(
                name: "AbpAuditLogActions",
                newName: "AbpAuditLogActions",
                newSchema: "ADM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OpenIddictTokens",
                schema: "ADM",
                newName: "OpenIddictTokens");

            migrationBuilder.RenameTable(
                name: "OpenIddictScopes",
                schema: "ADM",
                newName: "OpenIddictScopes");

            migrationBuilder.RenameTable(
                name: "OpenIddictAuthorizations",
                schema: "ADM",
                newName: "OpenIddictAuthorizations");

            migrationBuilder.RenameTable(
                name: "OpenIddictApplications",
                schema: "ADM",
                newName: "OpenIddictApplications");

            migrationBuilder.RenameTable(
                name: "AbpUserTokens",
                schema: "ADM",
                newName: "AbpUserTokens");

            migrationBuilder.RenameTable(
                name: "AbpUsers",
                schema: "ADM",
                newName: "AbpUsers");

            migrationBuilder.RenameTable(
                name: "AbpUserRoles",
                schema: "ADM",
                newName: "AbpUserRoles");

            migrationBuilder.RenameTable(
                name: "AbpUserOrganizationUnits",
                schema: "ADM",
                newName: "AbpUserOrganizationUnits");

            migrationBuilder.RenameTable(
                name: "AbpUserLogins",
                schema: "ADM",
                newName: "AbpUserLogins");

            migrationBuilder.RenameTable(
                name: "AbpUserDelegations",
                schema: "ADM",
                newName: "AbpUserDelegations");

            migrationBuilder.RenameTable(
                name: "AbpUserClaims",
                schema: "ADM",
                newName: "AbpUserClaims");

            migrationBuilder.RenameTable(
                name: "AbpTenants",
                schema: "ADM",
                newName: "AbpTenants");

            migrationBuilder.RenameTable(
                name: "AbpTenantConnectionStrings",
                schema: "ADM",
                newName: "AbpTenantConnectionStrings");

            migrationBuilder.RenameTable(
                name: "AbpSettings",
                schema: "ADM",
                newName: "AbpSettings");

            migrationBuilder.RenameTable(
                name: "AbpSettingDefinitions",
                schema: "ADM",
                newName: "AbpSettingDefinitions");

            migrationBuilder.RenameTable(
                name: "AbpSessions",
                schema: "ADM",
                newName: "AbpSessions");

            migrationBuilder.RenameTable(
                name: "AbpSecurityLogs",
                schema: "ADM",
                newName: "AbpSecurityLogs");

            migrationBuilder.RenameTable(
                name: "AbpRoles",
                schema: "ADM",
                newName: "AbpRoles");

            migrationBuilder.RenameTable(
                name: "AbpRoleClaims",
                schema: "ADM",
                newName: "AbpRoleClaims");

            migrationBuilder.RenameTable(
                name: "AbpPermissions",
                schema: "ADM",
                newName: "AbpPermissions");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGroups",
                schema: "ADM",
                newName: "AbpPermissionGroups");

            migrationBuilder.RenameTable(
                name: "AbpPermissionGrants",
                schema: "ADM",
                newName: "AbpPermissionGrants");

            migrationBuilder.RenameTable(
                name: "AbpOrganizationUnits",
                schema: "ADM",
                newName: "AbpOrganizationUnits");

            migrationBuilder.RenameTable(
                name: "AbpOrganizationUnitRoles",
                schema: "ADM",
                newName: "AbpOrganizationUnitRoles");

            migrationBuilder.RenameTable(
                name: "AbpLinkUsers",
                schema: "ADM",
                newName: "AbpLinkUsers");

            migrationBuilder.RenameTable(
                name: "AbpFeatureValues",
                schema: "ADM",
                newName: "AbpFeatureValues");

            migrationBuilder.RenameTable(
                name: "AbpFeatures",
                schema: "ADM",
                newName: "AbpFeatures");

            migrationBuilder.RenameTable(
                name: "AbpFeatureGroups",
                schema: "ADM",
                newName: "AbpFeatureGroups");

            migrationBuilder.RenameTable(
                name: "AbpEntityPropertyChanges",
                schema: "ADM",
                newName: "AbpEntityPropertyChanges");

            migrationBuilder.RenameTable(
                name: "AbpEntityChanges",
                schema: "ADM",
                newName: "AbpEntityChanges");

            migrationBuilder.RenameTable(
                name: "AbpClaimTypes",
                schema: "ADM",
                newName: "AbpClaimTypes");

            migrationBuilder.RenameTable(
                name: "AbpBlobs",
                schema: "ADM",
                newName: "AbpBlobs");

            migrationBuilder.RenameTable(
                name: "AbpBlobContainers",
                schema: "ADM",
                newName: "AbpBlobContainers");

            migrationBuilder.RenameTable(
                name: "AbpBackgroundJobs",
                schema: "ADM",
                newName: "AbpBackgroundJobs");

            migrationBuilder.RenameTable(
                name: "AbpAuditLogs",
                schema: "ADM",
                newName: "AbpAuditLogs");

            migrationBuilder.RenameTable(
                name: "AbpAuditLogExcelFiles",
                schema: "ADM",
                newName: "AbpAuditLogExcelFiles");

            migrationBuilder.RenameTable(
                name: "AbpAuditLogActions",
                schema: "ADM",
                newName: "AbpAuditLogActions");
        }
    }
}
