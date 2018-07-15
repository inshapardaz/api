using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class fixedgenreidspelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Book_GenereId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Genre_GenereId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookGenre",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropIndex(
                name: "IX_BookGenre_GenereId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropColumn(
                name: "GenereId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                schema: "Library",
                table: "BookGenre",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookGenre",
                schema: "Library",
                table: "BookGenre",
                columns: new[] { "BookId", "GenreId" });

            migrationBuilder.CreateIndex(
                name: "IX_BookGenre_GenreId",
                schema: "Library",
                table: "BookGenre",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Book_GenreId",
                schema: "Library",
                table: "BookGenre",
                column: "GenreId",
                principalSchema: "Library",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Genre_GenreId",
                schema: "Library",
                table: "BookGenre",
                column: "GenreId",
                principalSchema: "Library",
                principalTable: "Genre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Book_GenreId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropForeignKey(
                name: "FK_BookGenre_Genre_GenreId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookGenre",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropIndex(
                name: "IX_BookGenre_GenreId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.DropColumn(
                name: "GenreId",
                schema: "Library",
                table: "BookGenre");

            migrationBuilder.AddColumn<int>(
                name: "GenereId",
                schema: "Library",
                table: "BookGenre",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookGenre",
                schema: "Library",
                table: "BookGenre",
                columns: new[] { "BookId", "GenereId" });

            migrationBuilder.CreateIndex(
                name: "IX_BookGenre_GenereId",
                schema: "Library",
                table: "BookGenre",
                column: "GenereId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Book_GenereId",
                schema: "Library",
                table: "BookGenre",
                column: "GenereId",
                principalSchema: "Library",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookGenre_Genre_GenereId",
                schema: "Library",
                table: "BookGenre",
                column: "GenereId",
                principalSchema: "Library",
                principalTable: "Genre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
