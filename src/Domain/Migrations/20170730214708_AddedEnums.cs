using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Domain.Migrations
{
    public partial class AddedEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Word_Dictionary",
                schema: "Inshapardaz",
                table: "Word");

            migrationBuilder.AlterColumn<int>(
                name: "Language",
                schema: "Inshapardaz",
                table: "WordDetail",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DictionaryId",
                schema: "Inshapardaz",
                table: "Word",
                nullable: false,
                defaultValueSql: "1",
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValueSql: "1");

            migrationBuilder.AlterColumn<int>(
                name: "Language",
                schema: "Inshapardaz",
                table: "Dictionary",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                schema: "Inshapardaz",
                table: "Dictionary",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Word_Dictionary",
                schema: "Inshapardaz",
                table: "Word",
                column: "DictionaryId",
                principalSchema: "Inshapardaz",
                principalTable: "Dictionary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Word_Dictionary",
                schema: "Inshapardaz",
                table: "Word");

            migrationBuilder.AlterColumn<int>(
                name: "Language",
                schema: "Inshapardaz",
                table: "WordDetail",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DictionaryId",
                schema: "Inshapardaz",
                table: "Word",
                nullable: true,
                defaultValueSql: "1",
                oldClrType: typeof(int),
                oldDefaultValueSql: "1");

            migrationBuilder.AlterColumn<int>(
                name: "Language",
                schema: "Inshapardaz",
                table: "Dictionary",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<bool>(
                name: "IsPublic",
                schema: "Inshapardaz",
                table: "Dictionary",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddForeignKey(
                name: "FK_Word_Dictionary",
                schema: "Inshapardaz",
                table: "Word",
                column: "DictionaryId",
                principalSchema: "Inshapardaz",
                principalTable: "Dictionary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
