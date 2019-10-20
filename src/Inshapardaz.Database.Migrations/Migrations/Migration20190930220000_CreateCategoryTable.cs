using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930220000)]
    public class Migration20190930220000_CreateCategoryTable : Migration
    {
        public override void Up()
        {
            Create.Table("Category")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Category")
                .InSchema("Library");
        }
    }
}
