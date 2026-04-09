using LTC.AdministrationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    [DbContext(typeof(AdministrationServiceDbContext))]
    [Migration("20260409123000_RestoreOpenIddictTables")]
    public partial class RestoreOpenIddictTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[OpenIddictApplications]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OpenIddictApplications]
    (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationType] nvarchar(50) NULL,
        [ClientId] nvarchar(100) NULL,
        [ClientSecret] nvarchar(max) NULL,
        [ClientType] nvarchar(50) NULL,
        [ConsentType] nvarchar(50) NULL,
        [DisplayName] nvarchar(max) NULL,
        [DisplayNames] nvarchar(max) NULL,
        [JsonWebKeySet] nvarchar(max) NULL,
        [Permissions] nvarchar(max) NULL,
        [PostLogoutRedirectUris] nvarchar(max) NULL,
        [Properties] nvarchar(max) NULL,
        [RedirectUris] nvarchar(max) NULL,
        [Requirements] nvarchar(max) NULL,
        [Settings] nvarchar(max) NULL,
        [FrontChannelLogoutUri] nvarchar(max) NULL,
        [ClientUri] nvarchar(max) NULL,
        [LogoUri] nvarchar(max) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_OpenIddictApplications] PRIMARY KEY ([Id])
    );
END

IF OBJECT_ID(N'[dbo].[OpenIddictScopes]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OpenIddictScopes]
    (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NULL,
        [Descriptions] nvarchar(max) NULL,
        [DisplayName] nvarchar(max) NULL,
        [DisplayNames] nvarchar(max) NULL,
        [Name] nvarchar(200) NULL,
        [Properties] nvarchar(max) NULL,
        [Resources] nvarchar(max) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_OpenIddictScopes] PRIMARY KEY ([Id])
    );
END

IF OBJECT_ID(N'[dbo].[OpenIddictAuthorizations]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OpenIddictAuthorizations]
    (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NULL,
        [CreationDate] datetime2 NULL,
        [Properties] nvarchar(max) NULL,
        [Scopes] nvarchar(max) NULL,
        [Status] nvarchar(50) NULL,
        [Subject] nvarchar(400) NULL,
        [Type] nvarchar(50) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_OpenIddictAuthorizations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId]
            FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[OpenIddictApplications]([Id])
    );
END

IF OBJECT_ID(N'[dbo].[OpenIddictTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OpenIddictTokens]
    (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NULL,
        [AuthorizationId] uniqueidentifier NULL,
        [CreationDate] datetime2 NULL,
        [ExpirationDate] datetime2 NULL,
        [Payload] nvarchar(max) NULL,
        [Properties] nvarchar(max) NULL,
        [RedemptionDate] datetime2 NULL,
        [ReferenceId] nvarchar(100) NULL,
        [Status] nvarchar(50) NULL,
        [Subject] nvarchar(400) NULL,
        [Type] nvarchar(150) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_OpenIddictTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OpenIddictTokens_OpenIddictApplications_ApplicationId]
            FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[OpenIddictApplications]([Id]),
        CONSTRAINT [FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId]
            FOREIGN KEY ([AuthorizationId]) REFERENCES [dbo].[OpenIddictAuthorizations]([Id])
    );
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OpenIddictApplications_ClientId'
      AND object_id = OBJECT_ID(N'[dbo].[OpenIddictApplications]'))
BEGIN
    CREATE INDEX [IX_OpenIddictApplications_ClientId]
        ON [dbo].[OpenIddictApplications]([ClientId]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type'
      AND object_id = OBJECT_ID(N'[dbo].[OpenIddictAuthorizations]'))
BEGIN
    CREATE INDEX [IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type]
        ON [dbo].[OpenIddictAuthorizations]([ApplicationId], [Status], [Subject], [Type]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OpenIddictScopes_Name'
      AND object_id = OBJECT_ID(N'[dbo].[OpenIddictScopes]'))
BEGIN
    CREATE INDEX [IX_OpenIddictScopes_Name]
        ON [dbo].[OpenIddictScopes]([Name]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OpenIddictTokens_ApplicationId_Status_Subject_Type'
      AND object_id = OBJECT_ID(N'[dbo].[OpenIddictTokens]'))
BEGIN
    CREATE INDEX [IX_OpenIddictTokens_ApplicationId_Status_Subject_Type]
        ON [dbo].[OpenIddictTokens]([ApplicationId], [Status], [Subject], [Type]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OpenIddictTokens_AuthorizationId'
      AND object_id = OBJECT_ID(N'[dbo].[OpenIddictTokens]'))
BEGIN
    CREATE INDEX [IX_OpenIddictTokens_AuthorizationId]
        ON [dbo].[OpenIddictTokens]([AuthorizationId]);
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_OpenIddictTokens_ReferenceId'
      AND object_id = OBJECT_ID(N'[dbo].[OpenIddictTokens]'))
BEGIN
    CREATE INDEX [IX_OpenIddictTokens_ReferenceId]
        ON [dbo].[OpenIddictTokens]([ReferenceId]);
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally left empty to avoid accidental data loss on downgrade.
        }
    }
}