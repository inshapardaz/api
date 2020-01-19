using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddePageNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                schema: "Library",
                table: "BookPage",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNumber",
                schema: "Library",
                table: "BookPage");
        }
    }
}
