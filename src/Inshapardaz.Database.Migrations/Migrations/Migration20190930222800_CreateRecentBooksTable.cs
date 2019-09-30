using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930222800)]
    public class Migration20190930222800_CreateRecentBooksTable : Migration
    {
        public override void Up()
        {
            Create.Table("RecentBook")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("BookId").AsInt64()
                .WithColumn("UserId").AsGuid()
                .WithColumn("DateRead").AsDateTime();
            Create.ForeignKey()
                .FromTable("RecentBook").InSchema("Library").ForeignColumn("BookId")
                .ToTable("Book").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("RecentBook")
                .InSchema("Library");
        }
    }
}
