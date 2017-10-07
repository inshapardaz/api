using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Domain.Migrations
{
    public partial class RemovedWordDictionaryDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DictionaryId",
                schema: "Inshapardaz",
                table: "Word",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValueSql: "1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DictionaryId",
                schema: "Inshapardaz",
                table: "Word",
                nullable: false,
                defaultValueSql: "1",
                oldClrType: typeof(int));
        }
    }
}
