using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class UseUnixTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Projects"
                ALTER COLUMN "UpdatedDate" DROP DEFAULT,
                ALTER COLUMN "UpdatedDate" DROP NOT NULL,
                ALTER COLUMN "UpdatedDate" TYPE bigint
                USING NULL;

                ALTER TABLE "Projects"
                ALTER COLUMN "CreatedDate" DROP DEFAULT,
                ALTER COLUMN "CreatedDate" DROP NOT NULL,
                ALTER COLUMN "CreatedDate" TYPE bigint
                USING NULL;
            """);

            migrationBuilder.AlterColumn<long>(
                name: "UpdatedDate",
                table: "Projects",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedDate",
                table: "Projects",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
