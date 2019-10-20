using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930221800)]
    public class Migration20190930221800_CreateChapterTable : Migration
    {
        public override void Up()
        {
            Create.Table("Chapter")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("BookId").AsInt64()
                .WithColumn("ChapterNumber").AsInt32();
            Create.ForeignKey()
                .FromTable("Chapter").InSchema("Library").ForeignColumn("BookId")
                .ToTable("Book").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("Chapter")
                .InSchema("Library");
        }
    }
}
