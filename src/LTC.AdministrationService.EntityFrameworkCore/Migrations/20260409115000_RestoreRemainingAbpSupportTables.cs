using LTC.AdministrationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    [DbContext(typeof(AdministrationServiceDbContext))]
    [Migration("20260409115000_RestoreRemainingAbpSupportTables")]
    public partial class RestoreRemainingAbpSupportTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[AbpOrganizationUnitRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpOrganizationUnitRoles]
    (
        [RoleId] uniqueidentifier NOT NULL,
        [OrganizationUnitId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpOrganizationUnitRoles] PRIMARY KEY ([OrganizationUnitId], [RoleId]),
        CONSTRAINT [FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationUnitId]
            FOREIGN KEY ([OrganizationUnitId]) REFERENCES [dbo].[AbpOrganizationUnits] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpOrganizationUnitRoles_AbpRoles_RoleId]
            FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AbpRoles] ([Id]) ON DELETE CASCADE
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpOrganizationUnitRoles]'))
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId]
        ON [dbo].[AbpOrganizationUnitRoles]([RoleId], [OrganizationUnitId]);
END

IF OBJECT_ID(N'[dbo].[AbpSecurityLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpSecurityLogs]
    (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ApplicationName] nvarchar(96) NULL,
        [Identity] nvarchar(96) NULL,
        [Action] nvarchar(96) NULL,
        [UserId] uniqueidentifier NULL,
        [UserName] nvarchar(256) NULL,
        [TenantName] nvarchar(64) NULL,
        [ClientId] nvarchar(64) NULL,
        [CorrelationId] nvarchar(64) NULL,
        [ClientIpAddress] nvarchar(64) NULL,
        [BrowserInfo] nvarchar(512) NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpSecurityLogs] PRIMARY KEY ([Id])
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSecurityLogs_TenantId_Action'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSecurityLogs]'))
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_Action]
        ON [dbo].[AbpSecurityLogs]([TenantId], [Action]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSecurityLogs_TenantId_ApplicationName'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSecurityLogs]'))
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_ApplicationName]
        ON [dbo].[AbpSecurityLogs]([TenantId], [ApplicationName]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSecurityLogs_TenantId_Identity'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSecurityLogs]'))
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_Identity]
        ON [dbo].[AbpSecurityLogs]([TenantId], [Identity]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSecurityLogs_TenantId_UserId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSecurityLogs]'))
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_UserId]
        ON [dbo].[AbpSecurityLogs]([TenantId], [UserId]);
END

IF OBJECT_ID(N'[dbo].[AbpSessions]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpSessions]
    (
        [Id] uniqueidentifier NOT NULL,
        [SessionId] nvarchar(128) NOT NULL,
        [Device] nvarchar(64) NOT NULL,
        [DeviceInfo] nvarchar(256) NULL,
        [TenantId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ClientId] nvarchar(64) NULL,
        [IpAddresses] nvarchar(2048) NULL,
        [SignedIn] datetime2 NOT NULL,
        [LastAccessed] datetime2 NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpSessions] PRIMARY KEY ([Id])
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSessions_Device'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSessions]'))
BEGIN
    CREATE INDEX [IX_AbpSessions_Device] ON [dbo].[AbpSessions]([Device]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSessions_SessionId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSessions]'))
BEGIN
    CREATE INDEX [IX_AbpSessions_SessionId] ON [dbo].[AbpSessions]([SessionId]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpSessions_TenantId_UserId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpSessions]'))
BEGIN
    CREATE INDEX [IX_AbpSessions_TenantId_UserId]
        ON [dbo].[AbpSessions]([TenantId], [UserId]);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally left empty to avoid accidental data loss on downgrade.
        }
    }
}