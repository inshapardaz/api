using System;
using System.Data;
using FluentMigrator;

namespace Inshapardaz.Database.Migrations
{
    [Migration(000002)]
    public class Migration000002_Added_Issue_Status : Migration
    {
        public override void Up()
        {
            Alter.Table(Tables.Issue)
                .InSchema(Schemas.Library)
                .AddColumn("Status").AsInt32().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("Status")
                .FromTable(Tables.Issue)
                .InSchema(Schemas.Library);
        }
    }
}
