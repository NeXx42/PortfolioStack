using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTagReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_ProjectModel_ProjectId",
                table: "Elements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTagModel_ProjectModel_ProjectId",
                table: "ProjectTagModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTagModel_TagModel_TagId",
                table: "ProjectTagModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Releases_ProjectModel_ProjectId",
                table: "Releases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagModel",
                table: "TagModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTagModel",
                table: "ProjectTagModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectModel",
                table: "ProjectModel");

            migrationBuilder.RenameTable(
                name: "TagModel",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "ProjectTagModel",
                newName: "ProjectTags");

            migrationBuilder.RenameTable(
                name: "ProjectModel",
                newName: "Projects");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTagModel_TagId",
                table: "ProjectTags",
                newName: "IX_ProjectTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTagModel_ProjectId",
                table: "ProjectTags",
                newName: "IX_ProjectTags_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTags",
                table: "ProjectTags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_Projects_ProjectId",
                table: "Elements",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTags_Projects_ProjectId",
                table: "ProjectTags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTags_Tags_TagId",
                table: "ProjectTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_Projects_ProjectId",
                table: "Releases",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_Projects_ProjectId",
                table: "Elements");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTags_Projects_ProjectId",
                table: "ProjectTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTags_Tags_TagId",
                table: "ProjectTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Releases_Projects_ProjectId",
                table: "Releases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTags",
                table: "ProjectTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "TagModel");

            migrationBuilder.RenameTable(
                name: "ProjectTags",
                newName: "ProjectTagModel");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "ProjectModel");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTags_TagId",
                table: "ProjectTagModel",
                newName: "IX_ProjectTagModel_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTags_ProjectId",
                table: "ProjectTagModel",
                newName: "IX_ProjectTagModel_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagModel",
                table: "TagModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTagModel",
                table: "ProjectTagModel",
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
                name: "FK_ProjectTagModel_ProjectModel_ProjectId",
                table: "ProjectTagModel",
                column: "ProjectId",
                principalTable: "ProjectModel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTagModel_TagModel_TagId",
                table: "ProjectTagModel",
                column: "TagId",
                principalTable: "TagModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_ProjectModel_ProjectId",
                table: "Releases",
                column: "ProjectId",
                principalTable: "ProjectModel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
