using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateElementsLinkings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "elements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProjectElementParameterModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectElementId = table.Column<int>(type: "integer", nullable: false),
                    ParameterValue1 = table.Column<string>(type: "text", nullable: true),
                    ParameterValue2 = table.Column<string>(type: "text", nullable: true),
                    ParameterValue3 = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectElementParameterModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectElementParameterModel_elements_ProjectElementId",
                        column: x => x.ProjectElementId,
                        principalTable: "elements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectElementParameterModel_ProjectElementId",
                table: "ProjectElementParameterModel",
                column: "ProjectElementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectElementParameterModel");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "elements");
        }
    }
}
