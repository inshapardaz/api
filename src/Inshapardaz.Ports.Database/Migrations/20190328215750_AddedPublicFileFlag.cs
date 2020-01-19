using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddedPublicFileFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                schema: "Inshapardaz",
                table: "File",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                schema: "Inshapardaz",
                table: "File");
        }
    }
}
