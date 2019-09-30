using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190929235500)]
    public class Migration20190929235500_CreateFileTable : Migration
    {
        public override void Up()
        {
            Create.Table("File")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("DateCreated").AsDateTime()
                .WithColumn("FileName").AsString()
                .WithColumn("MimeType").AsString()
                .WithColumn("FileUrl").AsString()
                .WithColumn("IsPublic").AsBoolean();
        }

        public override void Down()
        {
            Delete.Table("File")
                .InSchema("Library");
        }
    }
}
