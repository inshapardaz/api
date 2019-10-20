using FluentMigrator;

namespace Inshapardaz.Database.Migrations.Migrations
{
    [Migration(20190930220100)]
    public class Migration20190930220100_CreateSeriesCategoryTable : Migration
    {
        public override void Up()
        {
            Create.Table("SeriesCategory")
                .InSchema("Library")
                .WithColumn("SeriesId").AsInt64()
                .WithColumn("CategoryId").AsInt64();

            Create.PrimaryKey("PK_SeriesCategory")
                .OnTable("SeriesCategory").WithSchema("Library")
                .Columns("SeriesId", "CategoryId");
            
            Create.ForeignKey()
                .FromTable("SeriesCategory").InSchema("Library").ForeignColumn("SeriesId")
                .ToTable("Series").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
            
            Create.ForeignKey()
                .FromTable("SeriesCategory").InSchema("Library").ForeignColumn("CategoryId")
                .ToTable("Category").InSchema("Library").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.Table("SeriesCategory")
                .InSchema("Library");
        }
    }
}
