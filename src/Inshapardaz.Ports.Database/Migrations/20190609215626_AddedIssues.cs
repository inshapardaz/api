using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddedIssues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Magazine_MagazineId",
                schema: "Library",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Magazine_MagazineCategory_CategoryId",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MagazineCategory",
                schema: "Library",
                table: "MagazineCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Magazine",
                schema: "Library",
                table: "Magazine");

            migrationBuilder.DropIndex(
                name: "IX_Issue_MagazineId",
                schema: "Library",
                table: "Issue");

            migrationBuilder.DropColumn(
                name: "MagazineId",
                schema: "Library",
                table: "Issue");

            migrationBuilder.RenameTable(
                name: "MagazineCategory",
                schema: "Library",
                newName: "PeriodicalCategory");

            migrationBuilder.RenameTable(
                name: "Magazine",
                schema: "Library",
                newName: "Periodical");

            migrationBuilder.RenameIndex(
                name: "IX_Magazine_CategoryId",
                schema: "Library",
                table: "Periodical",
                newName: "IX_Periodical_CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "PeriodicalId",
                schema: "Library",
                table: "Issue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PeriodicalCategory",
                schema: "Library",
                table: "PeriodicalCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Periodical",
                schema: "Library",
                table: "Periodical",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_PeriodicalId",
                schema: "Library",
                table: "Issue",
                column: "PeriodicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Periodical_PeriodicalId",
                schema: "Library",
                table: "Issue",
                column: "PeriodicalId",
                principalSchema: "Library",
                principalTable: "Periodical",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Periodical_PeriodicalCategory_CategoryId",
                schema: "Library",
                table: "Periodical",
                column: "CategoryId",
                principalSchema: "Library",
                principalTable: "PeriodicalCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Periodical_PeriodicalId",
                schema: "Library",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Periodical_PeriodicalCategory_CategoryId",
                schema: "Library",
                table: "Periodical");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PeriodicalCategory",
                schema: "Library",
                table: "PeriodicalCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Periodical",
                schema: "Library",
                table: "Periodical");

            migrationBuilder.DropIndex(
                name: "IX_Issue_PeriodicalId",
                schema: "Library",
                table: "Issue");

            migrationBuilder.DropColumn(
                name: "PeriodicalId",
                schema: "Library",
                table: "Issue");

            migrationBuilder.RenameTable(
                name: "PeriodicalCategory",
                schema: "Library",
                newName: "MagazineCategory");

            migrationBuilder.RenameTable(
                name: "Periodical",
                schema: "Library",
                newName: "Magazine");

            migrationBuilder.RenameIndex(
                name: "IX_Periodical_CategoryId",
                schema: "Library",
                table: "Magazine",
                newName: "IX_Magazine_CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "MagazineId",
                schema: "Library",
                table: "Issue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MagazineCategory",
                schema: "Library",
                table: "MagazineCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Magazine",
                schema: "Library",
                table: "Magazine",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_MagazineId",
                schema: "Library",
                table: "Issue",
                column: "MagazineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Magazine_MagazineId",
                schema: "Library",
                table: "Issue",
                column: "MagazineId",
                principalSchema: "Library",
                principalTable: "Magazine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Magazine_MagazineCategory_CategoryId",
                schema: "Library",
                table: "Magazine",
                column: "CategoryId",
                principalSchema: "Library",
                principalTable: "MagazineCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
