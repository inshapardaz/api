using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190929235600)]
    public class Migration20190929235600_CreateAuthorsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Author")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString().NotNullable().Indexed("IDX_AuthorName")
                .WithColumn("ImageId").AsInt64().Nullable();
            Create.ForeignKey()
                .FromTable("Author").InSchema("Library").ForeignColumn("ImageId")
                .ToTable("File").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.SetDefault);
        }

        public override void Down()
        {
            Delete.Table("Author")
                .InSchema("Library");
        }
    }
}
