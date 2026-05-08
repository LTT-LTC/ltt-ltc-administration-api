using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class ScreenUseSeatLayoutSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatLayout",
                schema: "dbo",
                table: "Screens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE s
                SET s.[SeatLayout] = m.[SeatLayout]
                FROM [dbo].[Screens] s
                INNER JOIN [dbo].[SeatMaps] m ON m.[Id] = s.[SeatMapId];
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_Screens_SeatMaps_SeatMapId",
                schema: "dbo",
                table: "Screens");

            migrationBuilder.DropIndex(
                name: "IX_Screens_SeatMapId",
                schema: "dbo",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "SeatMapId",
                schema: "dbo",
                table: "Screens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SeatMapId",
                schema: "dbo",
                table: "Screens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screens_SeatMapId",
                schema: "dbo",
                table: "Screens",
                column: "SeatMapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Screens_SeatMaps_SeatMapId",
                schema: "dbo",
                table: "Screens",
                column: "SeatMapId",
                principalSchema: "dbo",
                principalTable: "SeatMaps",
                principalColumn: "Id");

            migrationBuilder.DropColumn(
                name: "SeatLayout",
                schema: "dbo",
                table: "Screens");
        }
    }
}
