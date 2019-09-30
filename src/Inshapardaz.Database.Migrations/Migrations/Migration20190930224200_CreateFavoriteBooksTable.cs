using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930224200)]
    public class Migration20190930224200_CreateFavoriteBooksTable : Migration
    {
        public override void Up()
        {
            Create.Table("FavoriteBook")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("BookId").AsInt64()
                .WithColumn("UserId").AsGuid()
                .WithColumn("DateAdded").AsDateTime();
            Create.ForeignKey()
                .FromTable("FavoriteBook").InSchema("Library").ForeignColumn("BookId")
                .ToTable("Book").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("FavoriteBook")
                .InSchema("Library");
        }
    }
}
