using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    public partial class EmployeeCinemaAssignmentAlignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AvatarFileId", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "Code", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "DateOfBirth", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "Email", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "EmployeeId", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "IsFirstLogin", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "JoinedDate", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "Name", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "OrganizationUnitId", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "OtherEmail", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "PhoneNumber", schema: "dbo", table: "Employees");
            migrationBuilder.DropColumn(name: "PositionId", schema: "dbo", table: "Employees");

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'IX_Cinemas_ManagerUserId'
                    AND object_id = OBJECT_ID('[dbo].[Cinemas]')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Cinemas_ManagerUserId]
                    ON [dbo].[Cinemas]([ManagerUserId])
                    WHERE [ManagerUserId] IS NOT NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_Cinemas_AbpUsers_ManagerUserId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Cinemas]
                    ADD CONSTRAINT [FK_Cinemas_AbpUsers_ManagerUserId]
                    FOREIGN KEY ([ManagerUserId]) REFERENCES [dbo].[AbpUsers]([Id]) ON DELETE SET NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_Employees_AbpUsers_UserId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD CONSTRAINT [FK_Employees_AbpUsers_UserId]
                    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AbpUsers]([Id]) ON DELETE SET NULL;
                END
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_Employees_Cinemas_CinemaId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD CONSTRAINT [FK_Employees_Cinemas_CinemaId]
                    FOREIGN KEY ([CinemaId]) REFERENCES [dbo].[Cinemas]([Id]) ON DELETE SET NULL;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_Cinemas_CinemaId')
                    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_Cinemas_CinemaId];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_AbpUsers_UserId')
                    ALTER TABLE [dbo].[Employees] DROP CONSTRAINT [FK_Employees_AbpUsers_UserId];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Cinemas_AbpUsers_ManagerUserId')
                    ALTER TABLE [dbo].[Cinemas] DROP CONSTRAINT [FK_Cinemas_AbpUsers_ManagerUserId];
                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'IX_Cinemas_ManagerUserId'
                    AND object_id = OBJECT_ID('[dbo].[Cinemas]')
                )
                    DROP INDEX [IX_Cinemas_ManagerUserId] ON [dbo].[Cinemas];
                """);

            migrationBuilder.AddColumn<Guid>(name: "AvatarFileId", schema: "dbo", table: "Employees", type: "uniqueidentifier", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Code", schema: "dbo", table: "Employees", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<DateTime>(name: "DateOfBirth", schema: "dbo", table: "Employees", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Email", schema: "dbo", table: "Employees", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<string>(name: "EmployeeId", schema: "dbo", table: "Employees", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<bool>(name: "IsFirstLogin", schema: "dbo", table: "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<DateTime>(name: "JoinedDate", schema: "dbo", table: "Employees", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>(name: "Name", schema: "dbo", table: "Employees", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<Guid>(name: "OrganizationUnitId", schema: "dbo", table: "Employees", type: "uniqueidentifier", nullable: true);
            migrationBuilder.AddColumn<string>(name: "OtherEmail", schema: "dbo", table: "Employees", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<string>(name: "PhoneNumber", schema: "dbo", table: "Employees", type: "nvarchar(max)", nullable: true);
            migrationBuilder.AddColumn<Guid>(name: "PositionId", schema: "dbo", table: "Employees", type: "uniqueidentifier", nullable: true);
        }
    }
}
