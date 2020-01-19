using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class DeleteSeriesRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Series_SeriesId",
                schema: "Library",
                table: "Book");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Series_SeriesId",
                schema: "Library",
                table: "Book",
                column: "SeriesId",
                principalSchema: "Library",
                principalTable: "Series",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Series_SeriesId",
                schema: "Library",
                table: "Book");

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
    }
}
