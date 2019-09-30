using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190929000000)]
    public class Migration20190929000000_CreateLibrarySchema : Migration
    {
        public override void Up()
        {
            Create.Schema("Library");
        }

        public override void Down()
        {
            Delete.Schema("Library");
        }
    }
}
