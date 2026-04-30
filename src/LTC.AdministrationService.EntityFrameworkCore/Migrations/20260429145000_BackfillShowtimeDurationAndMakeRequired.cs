using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class BackfillShowtimeDurationAndMakeRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE s
                SET s.Duration = COALESCE(DATEDIFF(MINUTE, s.StartTime, s.EndTime), 0)
                FROM [dbo].[Showtimes] AS s
                WHERE s.Duration IS NULL OR s.Duration <= 0;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [dbo].[Showtimes]
                SET [Duration] = 0
                WHERE [Duration] IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                schema: "dbo",
                table: "Showtimes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                schema: "dbo",
                table: "Showtimes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
