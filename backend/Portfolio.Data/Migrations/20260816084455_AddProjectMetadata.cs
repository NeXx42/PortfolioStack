using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Projects",
                newName: "genre");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Releases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "genre",
                table: "Projects",
                newName: "Version");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Projects",
                type: "numeric",
                nullable: true);
        }
    }
}
