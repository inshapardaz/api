using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930221100)]
    public class Migration20190930221100_CreateBookCategoryTable : Migration
    {
        public override void Up()
        {
            Create.Table("BookCategory")
                .InSchema("Library")
                .WithColumn("BookId").AsInt64()
                .WithColumn("CategoryId").AsInt64();

            Create.PrimaryKey("PK_BookCategory")
                .OnTable("BookCategory").WithSchema("Library")
                .Columns("BookId", "CategoryId");
            Create.ForeignKey()
                .FromTable("BookCategory").InSchema("Library").ForeignColumn("CategoryId")
                .ToTable("Category").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
            Create.ForeignKey()
                .FromTable("BookCategory").InSchema("Library").ForeignColumn("BookId")
                .ToTable("Book").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("BookCategory")
                .InSchema("Library");
        }
    }
}
