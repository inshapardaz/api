using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Migrations
{
    public partial class AddedIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Inshapardaz",
                table: "Word",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Word_Title",
                schema: "Inshapardaz",
                table: "Word",
                column: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Word_Title",
                schema: "Inshapardaz",
                table: "Word");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Inshapardaz",
                table: "Word",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
