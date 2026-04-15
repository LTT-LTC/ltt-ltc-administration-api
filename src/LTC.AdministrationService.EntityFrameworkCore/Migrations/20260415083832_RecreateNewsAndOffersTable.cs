using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    /// <inheritdoc />
    public partial class RecreateNewsAndOffersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsAndOffers",
                table: "NewsAndOffers");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "NewsAndOffers",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "NewsAndOffers",
                newName: "startDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "NewsAndOffers",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "NewsAndOffers",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "NewsAndOffers",
                newName: "endDate");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "NewsAndOffers",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "CinemaId",
                table: "NewsAndOffers",
                newName: "cinemaId");

            migrationBuilder.AddColumn<Guid>(
                name: "newsAndOfferId",
                table: "NewsAndOffers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "NewsAndOffers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "posterUrl",
                table: "NewsAndOffers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updatedAt",
                table: "NewsAndOffers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsAndOffers",
                table: "NewsAndOffers",
                column: "newsAndOfferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsAndOffers",
                table: "NewsAndOffers");

            migrationBuilder.DropColumn(
                name: "newsAndOfferId",
                table: "NewsAndOffers");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "NewsAndOffers");

            migrationBuilder.DropColumn(
                name: "posterUrl",
                table: "NewsAndOffers");

            migrationBuilder.DropColumn(
                name: "updatedAt",
                table: "NewsAndOffers");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "NewsAndOffers",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "startDate",
                table: "NewsAndOffers",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "NewsAndOffers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "NewsAndOffers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "endDate",
                table: "NewsAndOffers",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "NewsAndOffers",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "cinemaId",
                table: "NewsAndOffers",
                newName: "CinemaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsAndOffers",
                table: "NewsAndOffers",
                column: "Id");
        }
    }
}
