using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930222500)]
    public class Migration20190930222500_CreateBookFileTable : Migration
    {
        public override void Up()
        {
            Create.Table("BookFile")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("BookId").AsInt64()
                .WithColumn("FileId").AsInt64();

            Create.ForeignKey()
                .FromTable("BookFile").InSchema("Library").ForeignColumn("BookId")
                .ToTable("Book").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
            Create.ForeignKey()
                .FromTable("BookFile").InSchema("Library").ForeignColumn("FileId")
                .ToTable("File").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("BookFile")
                .InSchema("Library");
        }
    }
}
