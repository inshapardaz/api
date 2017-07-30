using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inshapardaz.Domain.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Inshapardaz");

            migrationBuilder.CreateTable(
                name: "Dictionary",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsPublic = table.Column<bool>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contents = table.Column<byte[]>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LiveUntil = table.Column<DateTime>(nullable: false),
                    MimeType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Word",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    DictionaryId = table.Column<int>(nullable: true, defaultValue: 1),
                    Pronunciation = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TitleWithMovements = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Word_Dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryDownloads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DictionaryId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryDownloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictionaryDownloads_Dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DictionaryDownloads_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordDetail",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Attributes = table.Column<long>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    WordInstanceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordDetail_Word_WordInstanceId",
                        column: x => x.WordInstanceId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordRelation",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RelatedWordId = table.Column<long>(nullable: false),
                    RelationType = table.Column<int>(nullable: false),
                    SourceWordId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordRelation_Word_RelatedWordId",
                        column: x => x.RelatedWordId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordRelation_Word_SourceWordId",
                        column: x => x.SourceWordId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meaning",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Context = table.Column<string>(nullable: true),
                    Example = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    WordDetailId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meaning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meaning_WordDetail_WordDetailId",
                        column: x => x.WordDetailId,
                        principalSchema: "Inshapardaz",
                        principalTable: "WordDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translation",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Language = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    WordDetailId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translation_WordDetail_WordDetailId",
                        column: x => x.WordDetailId,
                        principalSchema: "Inshapardaz",
                        principalTable: "WordDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryDownloads_DictionaryId",
                table: "DictionaryDownloads",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryDownloads_FileId",
                table: "DictionaryDownloads",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Meaning_WordDetailId",
                schema: "Inshapardaz",
                table: "Meaning",
                column: "WordDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Translation_WordDetailId",
                schema: "Inshapardaz",
                table: "Translation",
                column: "WordDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Word_DictionaryId",
                schema: "Inshapardaz",
                table: "Word",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_WordDetail_WordInstanceId",
                schema: "Inshapardaz",
                table: "WordDetail",
                column: "WordInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WordRelation_RelatedWordId",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "RelatedWordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordRelation_SourceWordId",
                schema: "Inshapardaz",
                table: "WordRelation",
                column: "SourceWordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryDownloads");

            migrationBuilder.DropTable(
                name: "Meaning",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "Translation",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "WordRelation",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "WordDetail",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "Word",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "Dictionary",
                schema: "Inshapardaz");
        }
    }
}
