using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Domain.Migrations
{
    public partial class UpdatedForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meaning_WordDetail",
                schema: "Inshapardaz",
                table: "Meaning");

            migrationBuilder.DropForeignKey(
                name: "FK_Translation_WordDetail",
                schema: "Inshapardaz",
                table: "Translation");

            migrationBuilder.DropForeignKey(
                name: "FK_WordDetail_Word",
                schema: "Inshapardaz",
                table: "WordDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_Meaning_WordDetail",
                schema: "Inshapardaz",
                table: "Meaning",
                column: "WordDetailId",
                principalSchema: "Inshapardaz",
                principalTable: "WordDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translation_WordDetail",
                schema: "Inshapardaz",
                table: "Translation",
                column: "WordDetailId",
                principalSchema: "Inshapardaz",
                principalTable: "WordDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordDetail_Word",
                schema: "Inshapardaz",
                table: "WordDetail",
                column: "WordInstanceId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "RelatedWordId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meaning_WordDetail",
                schema: "Inshapardaz",
                table: "Meaning");

            migrationBuilder.DropForeignKey(
                name: "FK_Translation_WordDetail",
                schema: "Inshapardaz",
                table: "Translation");

            migrationBuilder.DropForeignKey(
                name: "FK_WordDetail_Word",
                schema: "Inshapardaz",
                table: "WordDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_Meaning_WordDetail",
                schema: "Inshapardaz",
                table: "Meaning",
                column: "WordDetailId",
                principalSchema: "Inshapardaz",
                principalTable: "WordDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Translation_WordDetail",
                schema: "Inshapardaz",
                table: "Translation",
                column: "WordDetailId",
                principalSchema: "Inshapardaz",
                principalTable: "WordDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WordDetail_Word",
                schema: "Inshapardaz",
                table: "WordDetail",
                column: "WordInstanceId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "RelatedWordId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
