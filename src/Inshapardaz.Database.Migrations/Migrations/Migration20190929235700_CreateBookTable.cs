using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190929235700)]
    public class Migration20190929235700_CreateBookTable : Migration
    {
        public override void Up()
        {
            Create.Table("Book")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Title").AsString().NotNullable().Indexed("IDX_Title")
                .WithColumn("Description").AsString()
                .WithColumn("AuthorId").AsInt64().NotNullable()
                .WithColumn("ImageId").AsInt64().Nullable()
                .WithColumn("IsPublic").AsBoolean()
                .WithColumn("Language").AsInt32()
                .WithColumn("Status").AsInt32()
                .WithColumn("SeriesId").AsInt64()
                .WithColumn("SeriesIndex").AsInt32()
                .WithColumn("Copyrights").AsInt32()
                .WithColumn("YearPublished").AsInt32()                
                .WithColumn("DateAdded").AsDateTime().NotNullable()
                .WithColumn("DateUpdated").AsDateTime();

            Create.ForeignKey()
                .FromTable("Book").InSchema("Library").ForeignColumn("AuthorId")
                .ToTable("Author").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            Create.ForeignKey()
                .FromTable("Book").InSchema("Library").ForeignColumn("ImageId")
                .ToTable("File").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.SetDefault);
        }

        public override void Down()
        {
            Delete.Table("Book")
                .InSchema("Library");
        }
    }
}
