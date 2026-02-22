using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormaliseTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProjectTagModel");

            migrationBuilder.DropColumn(
                name: "customColour",
                table: "ProjectTagModel");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "ProjectTagModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TagModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    customColour = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTagModel_TagId",
                table: "ProjectTagModel",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTagModel_TagModel_TagId",
                table: "ProjectTagModel",
                column: "TagId",
                principalTable: "TagModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTagModel_TagModel_TagId",
                table: "ProjectTagModel");

            migrationBuilder.DropTable(
                name: "TagModel");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTagModel_TagId",
                table: "ProjectTagModel");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "ProjectTagModel");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProjectTagModel",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customColour",
                table: "ProjectTagModel",
                type: "text",
                nullable: true);
        }
    }
}
