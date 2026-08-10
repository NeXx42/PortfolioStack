using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInReleaseEngine",
                table: "Releases");

            migrationBuilder.AddColumn<string>(
                name: "EntryPoint",
                table: "ReleaseDownloads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReleaseEngineManifest",
                table: "ReleaseDownloads",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryPoint",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "ReleaseEngineManifest",
                table: "ReleaseDownloads");

            migrationBuilder.AddColumn<bool>(
                name: "IsInReleaseEngine",
                table: "Releases",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
