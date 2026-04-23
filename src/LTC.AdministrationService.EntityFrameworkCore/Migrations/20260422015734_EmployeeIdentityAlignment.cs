using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeIdentityAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                schema: "dbo",
                table: "Employees",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                schema: "dbo",
                table: "Employees");
        }
    }
}
