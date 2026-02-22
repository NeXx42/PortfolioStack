using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrackElementParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectElementParameterModel_elements_ProjectElementId",
                table: "ProjectElementParameterModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectElementParameterModel",
                table: "ProjectElementParameterModel");

            migrationBuilder.RenameTable(
                name: "ProjectElementParameterModel",
                newName: "elementsParameters");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectElementParameterModel_ProjectElementId",
                table: "elementsParameters",
                newName: "IX_elementsParameters_ProjectElementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_elementsParameters",
                table: "elementsParameters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_elementsParameters_elements_ProjectElementId",
                table: "elementsParameters",
                column: "ProjectElementId",
                principalTable: "elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_elementsParameters_elements_ProjectElementId",
                table: "elementsParameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_elementsParameters",
                table: "elementsParameters");

            migrationBuilder.RenameTable(
                name: "elementsParameters",
                newName: "ProjectElementParameterModel");

            migrationBuilder.RenameIndex(
                name: "IX_elementsParameters_ProjectElementId",
                table: "ProjectElementParameterModel",
                newName: "IX_ProjectElementParameterModel_ProjectElementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectElementParameterModel",
                table: "ProjectElementParameterModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectElementParameterModel_elements_ProjectElementId",
                table: "ProjectElementParameterModel",
                column: "ProjectElementId",
                principalTable: "elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
