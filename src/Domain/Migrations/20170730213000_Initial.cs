using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inshapardaz.Domain.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "HangFire");

            migrationBuilder.EnsureSchema(
                name: "Inshapardaz");

            migrationBuilder.CreateTable(
                name: "AggregatedCounter",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpireAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedCounter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Counter",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpireAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dictionary",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsPublic = table.Column<bool>(nullable: true),
                    Language = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    UserId = table.Column<string>(maxLength: 50, nullable: true)
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contents = table.Column<byte[]>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LiveUntil = table.Column<DateTime>(nullable: false),
                    MimeType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hash",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpireAt = table.Column<DateTime>(nullable: true),
                    Field = table.Column<string>(maxLength: 100, nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hash", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Arguments = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvocationData = table.Column<string>(nullable: false),
                    StateId = table.Column<int>(nullable: true),
                    StateName = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobQueue",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FetchedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    JobId = table.Column<int>(nullable: false),
                    Queue = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "List",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpireAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_List", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schema",
                schema: "HangFire",
                columns: table => new
                {
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangFire_Schema", x => x.Version);
                });

            migrationBuilder.CreateTable(
                name: "Server",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    Data = table.Column<string>(nullable: true),
                    LastHeartbeat = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Server", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Set",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpireAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Score = table.Column<double>(nullable: false),
                    Value = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Set", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Word",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    DictionaryId = table.Column<int>(nullable: true, defaultValueSql: "1"),
                    Pronunciation = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TitleWithMovements = table.Column<string>(nullable: true)
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryDownload",
                schema: "Inshapardaz",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DictionaryId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false)
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
                        principalTable: "File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobParameter",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HangFire_JobParameter_Job",
                        column: x => x.JobId,
                        principalSchema: "HangFire",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "State",
                schema: "HangFire",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Data = table.Column<string>(nullable: true),
                    JobId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Reason = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HangFire_State_Job",
                        column: x => x.JobId,
                        principalSchema: "HangFire",
                        principalTable: "Job",
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
                    Language = table.Column<int>(nullable: true),
                    WordInstanceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordDetail_Word",
                        column: x => x.WordInstanceId,
                        principalSchema: "Inshapardaz",
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_Meaning_WordDetail",
                        column: x => x.WordDetailId,
                        principalSchema: "Inshapardaz",
                        principalTable: "WordDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_Translation_WordDetail",
                        column: x => x.WordDetailId,
                        principalSchema: "Inshapardaz",
                        principalTable: "WordDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_HangFire_CounterAggregated_Key",
                schema: "HangFire",
                table: "AggregatedCounter",
                columns: new[] { "Value", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Counter_Key",
                schema: "HangFire",
                table: "Counter",
                columns: new[] { "Value", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryDownload_DictionaryId",
                table: "DictionaryDownload",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryDownload_FileId",
                table: "DictionaryDownload",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Hash_Key",
                schema: "HangFire",
                table: "Hash",
                columns: new[] { "ExpireAt", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Hash_ExpireAt",
                schema: "HangFire",
                table: "Hash",
                columns: new[] { "Id", "ExpireAt" });

            migrationBuilder.CreateIndex(
                name: "UX_HangFire_Hash_Key_Field",
                schema: "HangFire",
                table: "Hash",
                columns: new[] { "Key", "Field" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Job_StateName",
                schema: "HangFire",
                table: "Job",
                column: "StateName");

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Job_ExpireAt",
                schema: "HangFire",
                table: "Job",
                columns: new[] { "Id", "ExpireAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_JobParameter_JobIdAndName",
                schema: "HangFire",
                table: "JobParameter",
                columns: new[] { "JobId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_JobQueue_QueueAndFetchedAt",
                schema: "HangFire",
                table: "JobQueue",
                columns: new[] { "Queue", "FetchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_List_ExpireAt",
                schema: "HangFire",
                table: "List",
                columns: new[] { "Id", "ExpireAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_List_Key",
                schema: "HangFire",
                table: "List",
                columns: new[] { "ExpireAt", "Value", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_Meaning_WordDetailId",
                schema: "Inshapardaz",
                table: "Meaning",
                column: "WordDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Set_ExpireAt",
                schema: "HangFire",
                table: "Set",
                columns: new[] { "Id", "ExpireAt" });

            migrationBuilder.CreateIndex(
                name: "UX_HangFire_Set_KeyAndValue",
                schema: "HangFire",
                table: "Set",
                columns: new[] { "Key", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_Set_Key",
                schema: "HangFire",
                table: "Set",
                columns: new[] { "ExpireAt", "Value", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_HangFire_State_JobId",
                schema: "HangFire",
                table: "State",
                column: "JobId");

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
                name: "AggregatedCounter",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "Counter",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "DictionaryDownload");

            migrationBuilder.DropTable(
                name: "Hash",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "JobParameter",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "JobQueue",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "List",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "Meaning",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "Schema",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "Server",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "Set",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "State",
                schema: "HangFire");

            migrationBuilder.DropTable(
                name: "Translation",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "WordRelation",
                schema: "Inshapardaz");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Job",
                schema: "HangFire");

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