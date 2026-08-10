using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReleaseFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseDownloads_Releases_ReleaseId",
                table: "ReleaseDownloads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Releases",
                table: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_Releases_ProjectId",
                table: "Releases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReleaseDownloads",
                table: "ReleaseDownloads");

            migrationBuilder.DropIndex(
                name: "IX_ReleaseDownloads_ReleaseId",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "DownloadType",
                table: "ReleaseDownloads");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Releases",
                newName: "VersionName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Releases",
                newName: "ReleaseId");

            migrationBuilder.RenameColumn(
                name: "DownloadLink",
                table: "ReleaseDownloads",
                newName: "DownloadUrl");

            migrationBuilder.AlterColumn<int>(
                name: "ReleaseId",
                table: "Releases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsInReleaseEngine",
                table: "Releases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PatchNotes",
                table: "Releases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "ReleaseDownloads",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "ReleaseDownloads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "DownloadSize",
                table: "ReleaseDownloads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Releases",
                table: "Releases",
                columns: new[] { "ProjectId", "ReleaseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReleaseDownloads",
                table: "ReleaseDownloads",
                columns: new[] { "ProjectId", "ReleaseId", "Platform" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseDownloads_Releases_ProjectId_ReleaseId",
                table: "ReleaseDownloads",
                columns: new[] { "ProjectId", "ReleaseId" },
                principalTable: "Releases",
                principalColumns: new[] { "ProjectId", "ReleaseId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseDownloads_ReleaseDownloads_ReleaseDownloadModelProje~",
                table: "ReleaseDownloads");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseDownloads_Releases_ProjectId_ReleaseId",
                table: "ReleaseDownloads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Releases",
                table: "Releases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReleaseDownloads",
                table: "ReleaseDownloads");

            migrationBuilder.DropIndex(
                name: "IX_ReleaseDownloads_ReleaseDownloadModelProjectId_ReleaseDownl~",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "IsInReleaseEngine",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "PatchNotes",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "ReleaseDownloads");

            migrationBuilder.DropColumn(
                name: "DownloadSize",
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

            migrationBuilder.RenameColumn(
                name: "VersionName",
                table: "Releases",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "ReleaseId",
                table: "Releases",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DownloadUrl",
                table: "ReleaseDownloads",
                newName: "DownloadLink");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Releases",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Releases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Releases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ReleaseDownloads",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "DownloadType",
                table: "ReleaseDownloads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Releases",
                table: "Releases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReleaseDownloads",
                table: "ReleaseDownloads",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ProjectId",
                table: "Releases",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseDownloads_ReleaseId",
                table: "ReleaseDownloads",
                column: "ReleaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseDownloads_Releases_ReleaseId",
                table: "ReleaseDownloads",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
