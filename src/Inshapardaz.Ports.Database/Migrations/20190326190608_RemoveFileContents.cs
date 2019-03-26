using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class RemoveFileContents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contents",
                schema: "Inshapardaz",
                table: "File");

            migrationBuilder.DropColumn(
                name: "LiveUntil",
                schema: "Inshapardaz",
                table: "File");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Contents",
                schema: "Inshapardaz",
                table: "File",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LiveUntil",
                schema: "Inshapardaz",
                table: "File",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
