using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class SplitScreenSeatMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeatMaps",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CinemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SeatLayout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeatCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatMaps", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "SeatMapId",
                schema: "dbo",
                table: "Screens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                """
                IF OBJECT_ID('tempdb..#ScreenSeatMap') IS NOT NULL DROP TABLE #ScreenSeatMap;
                CREATE TABLE #ScreenSeatMap (ScreenId uniqueidentifier NOT NULL PRIMARY KEY, SeatMapId uniqueidentifier NOT NULL);

                INSERT INTO #ScreenSeatMap (ScreenId, SeatMapId)
                SELECT [Id], NEWID() FROM [dbo].[Screens];

                INSERT INTO [dbo].[SeatMaps] ([Id], [TenantId], [CinemaId], [SeatLayout], [SeatCount], [CreatedAt], [UpdatedAt])
                SELECT m.[SeatMapId], s.[TenantId], s.[CinemaId], s.[SeatLayout], s.[SeatCount], s.[CreatedAt], s.[UpdatedAt]
                FROM [dbo].[Screens] s
                INNER JOIN #ScreenSeatMap m ON m.[ScreenId] = s.[Id];

                UPDATE s SET s.[SeatMapId] = m.[SeatMapId]
                FROM [dbo].[Screens] s
                INNER JOIN #ScreenSeatMap m ON m.[ScreenId] = s.[Id];

                DROP TABLE #ScreenSeatMap;
                """);

            migrationBuilder.DropColumn(
                name: "SeatCount",
                schema: "dbo",
                table: "Screens");

            migrationBuilder.DropColumn(
                name: "SeatLayout",
                schema: "dbo",
                table: "Screens");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screens_SeatMaps_SeatMapId",
                schema: "dbo",
                table: "Screens");

            migrationBuilder.DropIndex(
                name: "IX_Screens_SeatMapId",
                schema: "dbo",
                table: "Screens");

            migrationBuilder.AddColumn<int>(
                name: "SeatCount",
                schema: "dbo",
                table: "Screens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SeatLayout",
                schema: "dbo",
                table: "Screens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE s
                SET s.[SeatLayout] = m.[SeatLayout],
                    s.[SeatCount] = m.[SeatCount]
                FROM [dbo].[Screens] s
                INNER JOIN [dbo].[SeatMaps] m ON m.[Id] = s.[SeatMapId];
                """);

            migrationBuilder.DropColumn(
                name: "SeatMapId",
                schema: "dbo",
                table: "Screens");

            migrationBuilder.DropTable(
                name: "SeatMaps",
                schema: "dbo");
        }
    }
}
