using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "File",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contents = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LiveUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Word",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Attributes = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DictionaryId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Pronunciation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleWithMovements = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Word_Dictionary",
                        column: x => x.DictionaryId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryDownload",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DictionaryId = table.Column<int>(type: "int", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryDownload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictionaryDownload_Dictionary_DictionaryId",
                        column: x => x.DictionaryId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DictionaryDownload_File_FileId",
                        column: x => x.FileId,
                        principalSchema: "Inshapardaz",
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meaning",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Example = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WordId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meaning", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meaning_Word",
                        column: x => x.WordId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translation",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WordId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translation_Word",
                        column: x => x.WordId,
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RelatedWordId = table.Column<long>(type: "bigint", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    SourceWordId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordRelation_RelatedWord",
                        column: x => x.RelatedWordId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WordRelation_SourceWord",
                        column: x => x.SourceWordId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryDownload_DictionaryId",
                schema: "Inshapardaz",
                table: "DictionaryDownload",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryDownload_FileId",
                schema: "Inshapardaz",
                table: "DictionaryDownload",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Meaning_WordId",
                schema: "Inshapardaz",
                table: "Meaning",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Translation_WordId",
                schema: "Inshapardaz",
                table: "Translation",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Word_DictionaryId",
                schema: "Inshapardaz",
                table: "Word",
                column: "DictionaryId");

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
                name: "DictionaryDownload",
                schema: "Inshapardaz");

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
                name: "File",
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
