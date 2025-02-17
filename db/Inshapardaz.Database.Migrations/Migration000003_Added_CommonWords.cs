using System;
using System.Data;
using FluentMigrator;

namespace Inshapardaz.Database.Migrations
{
    [Migration(000003)]
    public class Migration000003_Added_CommonWords : Migration
    {
        public override void Up()
        {
            Create.Table("CommonWords")
                .InSchema(Schemas.Library)
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Language").AsString(4).NotNullable().WithDefaultValue("en")
                .WithColumn("Word").AsString(256).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("CommonWords")
                .InSchema(Schemas.Library);
        }
    }
}
