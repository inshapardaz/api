using System;
using System.Data;
using FluentMigrator;

namespace Inshapardaz.Database.Migrations
{
    [Migration(000004)]
    public class Migration000004_Added_ReadProgress : Migration
    {
        public override void Up()
        {
            Alter.Table("RecentBooks")
                .InSchema(Schemas.Library)
                .AddColumn("ProgressType").AsInt32().Nullable()
                .AddColumn("ProgressId").AsInt64().Nullable()
                .AddColumn("ProgressValue").AsDecimal().Nullable();
        }

        public override void Down()
        {
            Delete.Column("ProgressType")
                    .FromTable("RecentBooks")
                    .InSchema(Schemas.Library);
            Delete.Column("ProgressId")
                    .FromTable("RecentBooks")
                    .InSchema(Schemas.Library);
            Delete.Column("ProgressValue")
                    .FromTable("RecentBooks")
                    .InSchema(Schemas.Library);
        }
    }
}
