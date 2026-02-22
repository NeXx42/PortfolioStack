using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProperlyTrackTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_elements_projects_ProjectId",
                table: "elements");

            migrationBuilder.DropForeignKey(
                name: "FK_elementsParameters_elements_ProjectElementId",
                table: "elementsParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTagModel_projects_ProjectId",
                table: "ProjectTagModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_elementsParameters",
                table: "elementsParameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_elements",
                table: "elements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_projects",
                table: "projects");

            migrationBuilder.RenameTable(
                name: "elementsParameters",
                newName: "ElementsParameters");

            migrationBuilder.RenameTable(
                name: "elements",
                newName: "Elements");

            migrationBuilder.RenameTable(
                name: "projects",
                newName: "ProjectModel");

            migrationBuilder.RenameIndex(
                name: "IX_elementsParameters_ProjectElementId",
                table: "ElementsParameters",
                newName: "IX_ElementsParameters_ProjectElementId");

            migrationBuilder.RenameIndex(
                name: "IX_elements_ProjectId",
                table: "Elements",
                newName: "IX_Elements_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElementsParameters",
                table: "ElementsParameters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Elements",
                table: "Elements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectModel",
                table: "ProjectModel",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_ProjectModel_ProjectId",
                table: "Elements",
                column: "ProjectId",
                principalTable: "ProjectModel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElementsParameters_Elements_ProjectElementId",
                table: "ElementsParameters",
                column: "ProjectElementId",
                principalTable: "Elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTagModel_ProjectModel_ProjectId",
                table: "ProjectTagModel",
                column: "ProjectId",
                principalTable: "ProjectModel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_ProjectModel_ProjectId",
                table: "Elements");

            migrationBuilder.DropForeignKey(
                name: "FK_ElementsParameters_Elements_ProjectElementId",
                table: "ElementsParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTagModel_ProjectModel_ProjectId",
                table: "ProjectTagModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElementsParameters",
                table: "ElementsParameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Elements",
                table: "Elements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectModel",
                table: "ProjectModel");

            migrationBuilder.RenameTable(
                name: "ElementsParameters",
                newName: "elementsParameters");

            migrationBuilder.RenameTable(
                name: "Elements",
                newName: "elements");

            migrationBuilder.RenameTable(
                name: "ProjectModel",
                newName: "projects");

            migrationBuilder.RenameIndex(
                name: "IX_ElementsParameters_ProjectElementId",
                table: "elementsParameters",
                newName: "IX_elementsParameters_ProjectElementId");

            migrationBuilder.RenameIndex(
                name: "IX_Elements_ProjectId",
                table: "elements",
                newName: "IX_elements_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_elementsParameters",
                table: "elementsParameters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_elements",
                table: "elements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_projects",
                table: "projects",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_elements_projects_ProjectId",
                table: "elements",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_elementsParameters_elements_ProjectElementId",
                table: "elementsParameters",
                column: "ProjectElementId",
                principalTable: "elements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTagModel_projects_ProjectId",
                table: "ProjectTagModel",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
