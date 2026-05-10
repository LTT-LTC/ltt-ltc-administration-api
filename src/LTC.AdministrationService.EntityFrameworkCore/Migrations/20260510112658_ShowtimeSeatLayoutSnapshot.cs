using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class ShowtimeSeatLayoutSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatLayout",
                schema: "dbo",
                table: "Showtimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SeatMapId",
                schema: "dbo",
                table: "Showtimes",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatLayout",
                schema: "dbo",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "SeatMapId",
                schema: "dbo",
                table: "Showtimes");
        }
    }
}
