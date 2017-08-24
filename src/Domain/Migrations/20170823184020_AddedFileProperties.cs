using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Domain.Migrations
{
    public partial class AddedFileProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "Inshapardaz",
                table: "File",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                schema: "Inshapardaz",
                table: "DictionaryDownload",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "Inshapardaz",
                table: "File");

            migrationBuilder.DropColumn(
                name: "MimeType",
                schema: "Inshapardaz",
                table: "DictionaryDownload");
        }
    }
}
