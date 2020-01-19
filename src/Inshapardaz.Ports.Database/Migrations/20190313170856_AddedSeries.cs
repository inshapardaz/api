using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddedSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeriesId",
                schema: "Library",
                table: "Book",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeriesIndex",
                schema: "Library",
                table: "Book",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Series",
                schema: "Library",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeriesCategory",
                schema: "Library",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesCategory", x => new { x.SeriesId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_SeriesCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Library",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesCategory_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "Library",
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_SeriesId",
                schema: "Library",
                table: "Book",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesCategory_CategoryId",
                schema: "Library",
                table: "SeriesCategory",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Series_SeriesId",
                schema: "Library",
                table: "Book",
                column: "SeriesId",
                principalSchema: "Library",
                principalTable: "Series",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Series_SeriesId",
                schema: "Library",
                table: "Book");

            migrationBuilder.DropTable(
                name: "SeriesCategory",
                schema: "Library");

            migrationBuilder.DropTable(
                name: "Series",
                schema: "Library");

            migrationBuilder.DropIndex(
                name: "IX_Book_SeriesId",
                schema: "Library",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                schema: "Library",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "SeriesIndex",
                schema: "Library",
                table: "Book");
        }
    }
}
