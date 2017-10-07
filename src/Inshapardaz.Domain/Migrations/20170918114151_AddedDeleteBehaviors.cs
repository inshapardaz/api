using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Domain.Migrations
{
    public partial class AddedDeleteBehaviors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_WordRelation_SourceWord",
                schema: "Inshapardaz",
                table: "WordRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "RelatedWordId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WordRelation_SourceWord",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "SourceWordId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_WordRelation_SourceWord",
                schema: "Inshapardaz",
                table: "WordRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_WordRelation_RelatedWord",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "RelatedWordId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WordRelation_SourceWord",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "SourceWordId",
                principalSchema: "Inshapardaz",
                principalTable: "Word",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
