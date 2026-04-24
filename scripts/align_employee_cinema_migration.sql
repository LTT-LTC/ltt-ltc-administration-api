SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- 1) Align EF history with local baseline migrations so EF won't replay schema creation.
IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20260416060837_Initial')
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260416060837_Initial', '10.0.5');

IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20260421150632_AddShowtimeMovieProjection')
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260421150632_AddShowtimeMovieProjection', '10.0.5');

IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20260422010843_SplitScreenSeatMaps')
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260422010843_SplitScreenSeatMaps', '10.0.5');

-- 2) Apply only the EmployeeCinemaAssignmentAlignment schema updates.
IF COL_LENGTH('dbo.Employees', 'AvatarFileId') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN AvatarFileId;
IF COL_LENGTH('dbo.Employees', 'Code') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN Code;
IF COL_LENGTH('dbo.Employees', 'DateOfBirth') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN DateOfBirth;
IF COL_LENGTH('dbo.Employees', 'Email') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN Email;
IF COL_LENGTH('dbo.Employees', 'EmployeeId') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN EmployeeId;
IF COL_LENGTH('dbo.Employees', 'IsFirstLogin') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN IsFirstLogin;
IF COL_LENGTH('dbo.Employees', 'JoinedDate') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN JoinedDate;
IF COL_LENGTH('dbo.Employees', 'Name') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN Name;
IF COL_LENGTH('dbo.Employees', 'OrganizationUnitId') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN OrganizationUnitId;
IF COL_LENGTH('dbo.Employees', 'OtherEmail') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN OtherEmail;
IF COL_LENGTH('dbo.Employees', 'PhoneNumber') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN PhoneNumber;
IF COL_LENGTH('dbo.Employees', 'PositionId') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN PositionId;

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Cinemas_ManagerUserId'
      AND object_id = OBJECT_ID('dbo.Cinemas')
)
BEGIN
    CREATE UNIQUE INDEX IX_Cinemas_ManagerUserId
    ON dbo.Cinemas(ManagerUserId)
    WHERE ManagerUserId IS NOT NULL;
END

-- Clean orphan references before adding FKs.
UPDATE c
SET c.ManagerUserId = NULL
FROM dbo.Cinemas c
WHERE c.ManagerUserId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.AbpUsers u WHERE u.Id = c.ManagerUserId);

UPDATE e
SET e.UserId = NULL
FROM dbo.Employees e
WHERE e.UserId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.AbpUsers u WHERE u.Id = e.UserId);

UPDATE e
SET e.CinemaId = NULL
FROM dbo.Employees e
WHERE e.CinemaId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.Cinemas c WHERE c.Id = e.CinemaId);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Cinemas_AbpUsers_ManagerUserId')
BEGIN
    ALTER TABLE dbo.Cinemas
    ADD CONSTRAINT FK_Cinemas_AbpUsers_ManagerUserId
    FOREIGN KEY (ManagerUserId) REFERENCES dbo.AbpUsers(Id) ON DELETE SET NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_AbpUsers_UserId')
BEGIN
    ALTER TABLE dbo.Employees
    ADD CONSTRAINT FK_Employees_AbpUsers_UserId
    FOREIGN KEY (UserId) REFERENCES dbo.AbpUsers(Id) ON DELETE SET NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_Cinemas_CinemaId')
BEGIN
    ALTER TABLE dbo.Employees
    ADD CONSTRAINT FK_Employees_Cinemas_CinemaId
    FOREIGN KEY (CinemaId) REFERENCES dbo.Cinemas(Id) ON DELETE SET NULL;
END

IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20260424110000_EmployeeCinemaAssignmentAlignment')
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260424110000_EmployeeCinemaAssignmentAlignment', '10.0.5');

COMMIT TRANSACTION;
