using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDownload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseDownloads_ReleaseDownloads_ReleaseDownloadModelProje~",
                table: "ReleaseDownloads");

            migrationBuilder.DropIndex(
                name: "IX_ReleaseDownloads_ReleaseDownloadModelProjectId_ReleaseDownl~",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "ReleaseDownloadModelPlatform",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "ReleaseDownloadModelProjectId",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "ReleaseDownloadModelReleaseId",
                table: "ReleaseDownloads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReleaseDownloadModelPlatform",
                table: "ReleaseDownloads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseDownloadModelProjectId",
                table: "ReleaseDownloads",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseDownloadModelReleaseId",
                table: "ReleaseDownloads",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseDownloads_ReleaseDownloadModelProjectId_ReleaseDownl~",
                table: "ReleaseDownloads",
                columns: new[] { "ReleaseDownloadModelProjectId", "ReleaseDownloadModelReleaseId", "ReleaseDownloadModelPlatform" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseDownloads_ReleaseDownloads_ReleaseDownloadModelProje~",
                table: "ReleaseDownloads",
                columns: new[] { "ReleaseDownloadModelProjectId", "ReleaseDownloadModelReleaseId", "ReleaseDownloadModelPlatform" },
                principalTable: "ReleaseDownloads",
                principalColumns: new[] { "ProjectId", "ReleaseId", "Platform" });
        }
    }
}
