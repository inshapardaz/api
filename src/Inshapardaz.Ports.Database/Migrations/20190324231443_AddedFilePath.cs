using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddedFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                schema: "Inshapardaz",
                table: "File",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                schema: "Inshapardaz",
                table: "File");
        }
    }
}
