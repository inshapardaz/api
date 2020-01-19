using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class addedbookfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contents",
                schema: "Library",
                table: "BookPage");

            migrationBuilder.AddColumn<string>(
                name: "PageUrl",
                schema: "Library",
                table: "BookPage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "Library",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BookFile",
                schema: "Library",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookFile_Book_BookId",
                        column: x => x.BookId,
                        principalSchema: "Library",
                        principalTable: "Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookFile_File_FileId",
                        column: x => x.FileId,
                        principalSchema: "Inshapardaz",
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookFile_BookId",
                schema: "Library",
                table: "BookFile",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookFile_FileId",
                schema: "Library",
                table: "BookFile",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookFile",
                schema: "Library");

            migrationBuilder.DropColumn(
                name: "PageUrl",
                schema: "Library",
                table: "BookPage");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "Library",
                table: "Book");

            migrationBuilder.AddColumn<byte[]>(
                name: "Contents",
                schema: "Library",
                table: "BookPage",
                nullable: true);
        }
    }
}
