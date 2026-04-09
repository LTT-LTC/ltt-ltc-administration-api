using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using LTC.AdministrationService.EntityFrameworkCore;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    [DbContext(typeof(AdministrationServiceDbContext))]
    [Migration("20260409103000_RestoreMissingAbpTenantAndIdentityTables")]
    public partial class RestoreMissingAbpTenantAndIdentityTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[AbpLinkUsers]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpLinkUsers]
    (
        [Id] uniqueidentifier NOT NULL,
        [SourceUserId] uniqueidentifier NOT NULL,
        [SourceTenantId] uniqueidentifier NULL,
        [TargetUserId] uniqueidentifier NOT NULL,
        [TargetTenantId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpLinkUsers] PRIMARY KEY ([Id])
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpLinkUsers]'))
BEGIN
    CREATE UNIQUE INDEX [IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId]
        ON [dbo].[AbpLinkUsers]([SourceUserId], [SourceTenantId], [TargetUserId], [TargetTenantId])
        WHERE [SourceTenantId] IS NOT NULL AND [TargetTenantId] IS NOT NULL;
END

IF OBJECT_ID(N'[dbo].[AbpRoleClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpRoleClaims]
    (
        [Id] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1024) NULL,
        CONSTRAINT [PK_AbpRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpRoleClaims_AbpRoles_RoleId]
            FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AbpRoles] ([Id]) ON DELETE CASCADE
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpRoleClaims_RoleId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpRoleClaims]'))
BEGIN
    CREATE INDEX [IX_AbpRoleClaims_RoleId] ON [dbo].[AbpRoleClaims]([RoleId]);
END

IF OBJECT_ID(N'[dbo].[AbpTenantConnectionStrings]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpTenantConnectionStrings]
    (
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(64) NOT NULL,
        [Value] nvarchar(1024) NOT NULL,
        CONSTRAINT [PK_AbpTenantConnectionStrings] PRIMARY KEY ([TenantId], [Name]),
        CONSTRAINT [FK_AbpTenantConnectionStrings_AbpTenants_TenantId]
            FOREIGN KEY ([TenantId]) REFERENCES [dbo].[AbpTenants] ([Id]) ON DELETE CASCADE
    );
END

IF OBJECT_ID(N'[dbo].[AbpUserClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpUserClaims]
    (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1024) NULL,
        CONSTRAINT [PK_AbpUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpUserClaims_AbpUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AbpUsers] ([Id]) ON DELETE CASCADE
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpUserClaims_UserId'
      AND object_id = OBJECT_ID(N'[dbo].[AbpUserClaims]'))
BEGIN
    CREATE INDEX [IX_AbpUserClaims_UserId] ON [dbo].[AbpUserClaims]([UserId]);
END

IF OBJECT_ID(N'[dbo].[AbpUserLogins]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpUserLogins]
    (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(64) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ProviderKey] nvarchar(196) NOT NULL,
        [ProviderDisplayName] nvarchar(128) NULL,
        CONSTRAINT [PK_AbpUserLogins] PRIMARY KEY ([UserId], [LoginProvider]),
        CONSTRAINT [FK_AbpUserLogins_AbpUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AbpUsers] ([Id]) ON DELETE CASCADE
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_AbpUserLogins_LoginProvider_ProviderKey'
      AND object_id = OBJECT_ID(N'[dbo].[AbpUserLogins]'))
BEGIN
    CREATE INDEX [IX_AbpUserLogins_LoginProvider_ProviderKey]
        ON [dbo].[AbpUserLogins]([LoginProvider], [ProviderKey]);
END

IF OBJECT_ID(N'[dbo].[AbpUserTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AbpUserTokens]
    (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(64) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AbpUserTokens_AbpUsers_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [dbo].[AbpUsers] ([Id]) ON DELETE CASCADE
    );
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally left empty to avoid accidental data loss on downgrade.
        }
    }
}