IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpClaimTypes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(256) NOT NULL,
        [Required] bit NOT NULL,
        [IsStatic] bit NOT NULL,
        [Regex] nvarchar(512) NULL,
        [RegexDescription] nvarchar(128) NULL,
        [Description] nvarchar(256) NULL,
        [ValueType] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpClaimTypes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpFeatureGroups] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpFeatureGroups] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpFeatures] (
        [Id] uniqueidentifier NOT NULL,
        [GroupName] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [ParentName] nvarchar(128) NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [Description] nvarchar(256) NULL,
        [DefaultValue] nvarchar(256) NULL,
        [IsVisibleToClients] bit NOT NULL,
        [IsAvailableToHost] bit NOT NULL,
        [AllowedProviders] nvarchar(256) NULL,
        [ValueType] nvarchar(2048) NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpFeatures] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpFeatureValues] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(128) NOT NULL,
        [ProviderName] nvarchar(64) NULL,
        [ProviderKey] nvarchar(64) NULL,
        CONSTRAINT [PK_AbpFeatureValues] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpLinkUsers] (
        [Id] uniqueidentifier NOT NULL,
        [SourceUserId] uniqueidentifier NOT NULL,
        [SourceTenantId] uniqueidentifier NULL,
        [TargetUserId] uniqueidentifier NOT NULL,
        [TargetTenantId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpLinkUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpOrganizationUnits] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ParentId] uniqueidentifier NULL,
        [Code] nvarchar(95) NOT NULL,
        [DisplayName] nvarchar(128) NOT NULL,
        [EntityVersion] int NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AbpOrganizationUnits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [AbpOrganizationUnits] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpPermissionGrants] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(128) NOT NULL,
        [ProviderName] nvarchar(64) NOT NULL,
        [ProviderKey] nvarchar(64) NOT NULL,
        CONSTRAINT [PK_AbpPermissionGrants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpPermissionGroups] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpPermissionGroups] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpPermissions] (
        [Id] uniqueidentifier NOT NULL,
        [GroupName] nvarchar(128) NULL,
        [Name] nvarchar(128) NOT NULL,
        [ResourceName] nvarchar(256) NULL,
        [ManagementPermissionName] nvarchar(128) NULL,
        [ParentName] nvarchar(128) NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [IsEnabled] bit NOT NULL,
        [MultiTenancySide] tinyint NOT NULL,
        [Providers] nvarchar(128) NULL,
        [StateCheckers] nvarchar(256) NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpPermissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpResourcePermissionGrants] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(128) NOT NULL,
        [ProviderName] nvarchar(64) NOT NULL,
        [ProviderKey] nvarchar(64) NOT NULL,
        [ResourceName] nvarchar(256) NOT NULL,
        [ResourceKey] nvarchar(256) NOT NULL,
        CONSTRAINT [PK_AbpResourcePermissionGrants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpRoles] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(256) NOT NULL,
        [NormalizedName] nvarchar(256) NOT NULL,
        [IsDefault] bit NOT NULL,
        [IsStatic] bit NOT NULL,
        [IsPublic] bit NOT NULL,
        [EntityVersion] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpSecurityLogs] (
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
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpSessions] (
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
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpSettingDefinitions] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [Description] nvarchar(512) NULL,
        [DefaultValue] nvarchar(2048) NULL,
        [IsVisibleToClients] bit NOT NULL,
        [Providers] nvarchar(1024) NULL,
        [IsInherited] bit NOT NULL,
        [IsEncrypted] bit NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpSettingDefinitions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(2048) NOT NULL,
        [ProviderName] nvarchar(64) NULL,
        [ProviderKey] nvarchar(64) NULL,
        CONSTRAINT [PK_AbpSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpTenants] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(64) NOT NULL,
        [NormalizedName] nvarchar(64) NOT NULL,
        [EntityVersion] int NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AbpTenants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserDelegations] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [SourceUserId] uniqueidentifier NOT NULL,
        [TargetUserId] uniqueidentifier NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        CONSTRAINT [PK_AbpUserDelegations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUsers] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [UserName] nvarchar(256) NOT NULL,
        [NormalizedUserName] nvarchar(256) NOT NULL,
        [Name] nvarchar(64) NULL,
        [Surname] nvarchar(64) NULL,
        [Email] nvarchar(256) NOT NULL,
        [NormalizedEmail] nvarchar(256) NOT NULL,
        [EmailConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PasswordHash] nvarchar(256) NULL,
        [SecurityStamp] nvarchar(256) NOT NULL,
        [IsExternal] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PhoneNumber] nvarchar(16) NULL,
        [PhoneNumberConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit),
        [IsActive] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL DEFAULT CAST(0 AS bit),
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL DEFAULT CAST(0 AS bit),
        [AccessFailedCount] int NOT NULL DEFAULT 0,
        [ShouldChangePasswordOnNextLogin] bit NOT NULL,
        [EntityVersion] int NOT NULL,
        [LastPasswordChangeTime] datetimeoffset NULL,
        [LastSignInTime] datetimeoffset NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AbpUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [CinemaAmenities] (
        [Id] uniqueidentifier NOT NULL,
        [CinemaId] uniqueidentifier NOT NULL,
        [AmenitiesTypeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CinemaAmenities] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [CinemaAmenityTypes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Icon] nvarchar(max) NULL,
        CONSTRAINT [PK_CinemaAmenityTypes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [Cinemas] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NULL,
        [Ward] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [ManagerUserId] uniqueidentifier NULL,
        [ServiceNumber] nvarchar(max) NULL,
        [Status] nvarchar(max) NULL,
        [CreatedAt] datetime2 NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Cinemas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] uniqueidentifier NOT NULL,
        [EmployeeId] nvarchar(max) NULL,
        [TenantId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NULL,
        [CinemaId] uniqueidentifier NULL,
        [Scope] nvarchar(max) NULL,
        [Position] nvarchar(max) NULL,
        [HireDate] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        [ManagedByEmployeeId] uniqueidentifier NULL,
        [CreatedByUserId] uniqueidentifier NULL,
        [Name] nvarchar(max) NULL,
        [Code] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [OtherEmail] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [OrganizationUnitId] uniqueidentifier NULL,
        [PositionId] uniqueidentifier NULL,
        [AvatarFileId] uniqueidentifier NULL,
        [JoinedDate] datetime2 NULL,
        [DateOfBirth] datetime2 NULL,
        [IsFirstLogin] bit NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [GiftCodes] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [DiscountType] nvarchar(max) NULL,
        [DiscountValue] decimal(18,2) NOT NULL,
        [MinOrderAmount] decimal(18,2) NULL,
        [UsageLimit] int NULL,
        [UsageCount] int NOT NULL,
        [PerUserLimit] int NULL,
        [StartDate] datetime2 NULL,
        [EndDate] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        CONSTRAINT [PK_GiftCodes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [MediaFiles] (
        [Id] uniqueidentifier NOT NULL,
        [DisplayName] nvarchar(max) NOT NULL,
        [ResourceType] nvarchar(max) NOT NULL,
        [SecureUrl] nvarchar(max) NOT NULL,
        [PublicId] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [AssetId] nvarchar(max) NOT NULL,
        [Format] nvarchar(max) NOT NULL,
        [Size] bigint NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_MediaFiles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [PricingRules] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CinemaId] uniqueidentifier NOT NULL,
        [SeatTypeId] uniqueidentifier NULL,
        [RuleType] nvarchar(max) NULL,
        [Multiplier] decimal(18,2) NOT NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [DayOfWeek] int NULL,
        [Priority] int NOT NULL,
        [ValidFrom] datetime2 NULL,
        [ValidUntil] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_PricingRules] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [RevenueSnapshots] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CinemaId] uniqueidentifier NOT NULL,
        [SnapshotDate] datetime2 NOT NULL,
        [Granularity] nvarchar(max) NULL,
        [TotalRevenue] decimal(18,2) NOT NULL,
        [TotalBookings] int NOT NULL,
        [TotalTickets] int NOT NULL,
        [OccupancyRate] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NULL,
        CONSTRAINT [PK_RevenueSnapshots] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [Screens] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CinemaId] uniqueidentifier NOT NULL,
        [ScreenNumber] int NOT NULL,
        [ScreenType] nvarchar(max) NULL,
        [SeatLayout] nvarchar(max) NULL,
        [SeatCount] int NOT NULL,
        [CreatedAt] datetime2 NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Screens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [SeatTypes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [NumberOfSeat] int NOT NULL,
        [DisplayDirection] nvarchar(max) NULL,
        [PriceMultiplier] decimal(18,2) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SeatTypes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [Showtimes] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CinemaId] uniqueidentifier NOT NULL,
        [DistributionId] uniqueidentifier NOT NULL,
        [ScreenId] uniqueidentifier NOT NULL,
        [ShowDate] datetime2 NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [BasePrice] decimal(18,2) NOT NULL,
        [Status] nvarchar(max) NULL,
        [CreatedAt] datetime2 NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Showtimes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpOrganizationUnitRoles] (
        [RoleId] uniqueidentifier NOT NULL,
        [OrganizationUnitId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpOrganizationUnitRoles] PRIMARY KEY ([OrganizationUnitId], [RoleId]),
        CONSTRAINT [FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationUnitId] FOREIGN KEY ([OrganizationUnitId]) REFERENCES [AbpOrganizationUnits] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpOrganizationUnitRoles_AbpRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AbpRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpRoleClaims] (
        [Id] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1024) NULL,
        CONSTRAINT [PK_AbpRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpRoleClaims_AbpRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AbpRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpTenantConnectionStrings] (
        [TenantId] uniqueidentifier NOT NULL,
        [Name] nvarchar(64) NOT NULL,
        [Value] nvarchar(1024) NOT NULL,
        CONSTRAINT [PK_AbpTenantConnectionStrings] PRIMARY KEY ([TenantId], [Name]),
        CONSTRAINT [FK_AbpTenantConnectionStrings_AbpTenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [AbpTenants] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserClaims] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1024) NULL,
        CONSTRAINT [PK_AbpUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpUserClaims_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserLogins] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(64) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ProviderKey] nvarchar(196) NOT NULL,
        [ProviderDisplayName] nvarchar(128) NULL,
        CONSTRAINT [PK_AbpUserLogins] PRIMARY KEY ([UserId], [LoginProvider]),
        CONSTRAINT [FK_AbpUserLogins_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserOrganizationUnits] (
        [UserId] uniqueidentifier NOT NULL,
        [OrganizationUnitId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpUserOrganizationUnits] PRIMARY KEY ([OrganizationUnitId], [UserId]),
        CONSTRAINT [FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationUnitId] FOREIGN KEY ([OrganizationUnitId]) REFERENCES [AbpOrganizationUnits] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpUserOrganizationUnits_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserPasskeys] (
        [CredentialId] varbinary(1024) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Data] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpUserPasskeys] PRIMARY KEY ([CredentialId]),
        CONSTRAINT [FK_AbpUserPasskeys_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserPasswordHistories] (
        [UserId] uniqueidentifier NOT NULL,
        [Password] nvarchar(256) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_AbpUserPasswordHistories] PRIMARY KEY ([UserId], [Password]),
        CONSTRAINT [FK_AbpUserPasswordHistories_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AbpUserRoles_AbpRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AbpRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpUserRoles_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE TABLE [AbpUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(64) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AbpUserTokens_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpFeatureGroups_Name] ON [AbpFeatureGroups] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpFeatures_GroupName] ON [AbpFeatures] ([GroupName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpFeatures_Name] ON [AbpFeatures] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpFeatureValues_Name_ProviderName_ProviderKey] ON [AbpFeatureValues] ([Name], [ProviderName], [ProviderKey]) WHERE [ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId] ON [AbpLinkUsers] ([SourceUserId], [SourceTenantId], [TargetUserId], [TargetTenantId]) WHERE [SourceTenantId] IS NOT NULL AND [TargetTenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId] ON [AbpOrganizationUnitRoles] ([RoleId], [OrganizationUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnits_Code] ON [AbpOrganizationUnits] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnits_ParentId] ON [AbpOrganizationUnits] ([ParentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey] ON [AbpPermissionGrants] ([TenantId], [Name], [ProviderName], [ProviderKey]) WHERE [TenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpPermissionGroups_Name] ON [AbpPermissionGroups] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpPermissions_GroupName] ON [AbpPermissions] ([GroupName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpPermissions_ResourceName_Name] ON [AbpPermissions] ([ResourceName], [Name]) WHERE [ResourceName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpResourcePermissionGrants_TenantId_Name_ResourceName_ResourceKey_ProviderName_ProviderKey] ON [AbpResourcePermissionGrants] ([TenantId], [Name], [ResourceName], [ResourceKey], [ProviderName], [ProviderKey]) WHERE [TenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpRoleClaims_RoleId] ON [AbpRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpRoles_NormalizedName] ON [AbpRoles] ([NormalizedName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_Action] ON [AbpSecurityLogs] ([TenantId], [Action]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_ApplicationName] ON [AbpSecurityLogs] ([TenantId], [ApplicationName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_Identity] ON [AbpSecurityLogs] ([TenantId], [Identity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_UserId] ON [AbpSecurityLogs] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSessions_Device] ON [AbpSessions] ([Device]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSessions_SessionId] ON [AbpSessions] ([SessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSessions_TenantId_UserId] ON [AbpSessions] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpSettingDefinitions_Name] ON [AbpSettingDefinitions] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpSettings_Name_ProviderName_ProviderKey] ON [AbpSettings] ([Name], [ProviderName], [ProviderKey]) WHERE [ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpTenants_Name] ON [AbpTenants] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpTenants_NormalizedName] ON [AbpTenants] ([NormalizedName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserClaims_UserId] ON [AbpUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserLogins_LoginProvider_ProviderKey] ON [AbpUserLogins] ([LoginProvider], [ProviderKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId] ON [AbpUserOrganizationUnits] ([UserId], [OrganizationUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserPasskeys_UserId] ON [AbpUserPasskeys] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserRoles_RoleId_UserId] ON [AbpUserRoles] ([RoleId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_Email] ON [AbpUsers] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_NormalizedEmail] ON [AbpUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_NormalizedUserName] ON [AbpUsers] ([NormalizedUserName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_UserName] ON [AbpUsers] ([UserName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408160702_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260408160702_Initial', N'10.0.5');
END;

COMMIT;
GO

