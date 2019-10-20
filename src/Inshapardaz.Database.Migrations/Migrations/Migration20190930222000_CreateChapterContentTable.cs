using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930222000)]
    public class Migration20190930222000_CreateChapterContentTable : Migration
    {
        public override void Up()
        {
            Create.Table("ChapterContent")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("ContentUrl").AsString()
                .WithColumn("ChapterId").AsInt64()
                .WithColumn("MimeType").AsString();
            Create.ForeignKey()
                .FromTable("ChapterContent").InSchema("Library").ForeignColumn("ChapterId")
                .ToTable("Chapter").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("ChapterContent")
                .InSchema("Library");
        }
    }
}
