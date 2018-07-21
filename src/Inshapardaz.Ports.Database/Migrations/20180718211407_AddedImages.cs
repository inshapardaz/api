using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Ports.Database.Migrations
{
    public partial class AddedImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_BookImage_ImageId",
                schema: "Library",
                table: "Book");

            migrationBuilder.DropTable(
                name: "BookImage",
                schema: "Library");

            migrationBuilder.DropIndex(
                name: "IX_Book_ImageId",
                schema: "Library",
                table: "Book");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                schema: "Library",
                table: "Book",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                schema: "Library",
                table: "Author",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                schema: "Library",
                table: "Author");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                schema: "Library",
                table: "Book",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "BookImage",
                schema: "Library",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Image = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookImage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_ImageId",
                schema: "Library",
                table: "Book",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_BookImage_ImageId",
                schema: "Library",
                table: "Book",
                column: "ImageId",
                principalSchema: "Library",
                principalTable: "BookImage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
