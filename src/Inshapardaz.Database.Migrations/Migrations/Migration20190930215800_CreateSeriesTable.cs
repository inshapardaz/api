using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930215800)]
    public class Migration20190930215800_CreateSeriesTable : Migration
    {
        public override void Up()
        {
            Create.Table("Series")
                .InSchema("Library")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString().NotNullable().Indexed("IDX_Name")
                .WithColumn("ImageId").AsInt64().Nullable();

            Create.ForeignKey()
                .FromTable("Series").InSchema("Library").ForeignColumn("ImageId")
                .ToTable("File").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.SetDefault);
        }

        public override void Down()
        {
            Delete.Table("Series")
                .InSchema("Library");
        }
    }
}
