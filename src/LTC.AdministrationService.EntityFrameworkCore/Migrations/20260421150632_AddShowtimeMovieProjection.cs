using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class AddShowtimeMovieProjection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieFormat",
                schema: "dbo",
                table: "Showtimes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MovieId",
                schema: "dbo",
                table: "Showtimes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "CinemaAmenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "CinemaAmenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "dbo",
                table: "CinemaAmenities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "CinemaAmenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MovieDistributionProjections",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieDistributionProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieProjections",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieProjections", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieDistributionProjections",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "MovieProjections",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "MovieFormat",
                schema: "dbo",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "MovieId",
                schema: "dbo",
                table: "Showtimes");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "CinemaAmenities");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "dbo",
                table: "CinemaAmenities");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "dbo",
                table: "CinemaAmenities");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "CinemaAmenities");
        }
    }
}
