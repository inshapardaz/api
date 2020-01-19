using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddedPeriodicals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Magazine_MagazineCategory_MagazineCategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropIndex(
                name: "IX_Magazine_MagazineCategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropColumn(
                name: "MagazineCategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                schema: "Library",
                table: "Magazine",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Magazine_CategoryId",
                schema: "Library",
                table: "Magazine",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Magazine_MagazineCategory_CategoryId",
                schema: "Library",
                table: "Magazine",
                column: "CategoryId",
                principalSchema: "Library",
                principalTable: "MagazineCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Magazine_MagazineCategory_CategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropIndex(
                name: "IX_Magazine_CategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                schema: "Library",
                table: "Magazine",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MagazineCategoryId",
                schema: "Library",
                table: "Magazine",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Magazine_MagazineCategoryId",
                schema: "Library",
                table: "Magazine",
                column: "MagazineCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Magazine_MagazineCategory_MagazineCategoryId",
                schema: "Library",
                table: "Magazine",
                column: "MagazineCategoryId",
                principalSchema: "Library",
                principalTable: "MagazineCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
