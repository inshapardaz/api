using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000011)]
public class Migration000011_AddCategoryHierarchy : Migration
{
    public override void Up()
    {
        Alter.Table(Tables.Category).InSchema(Schemas.Library)
            .AddColumn("ParentCategoryId").AsInt32().Nullable()
                .Indexed("IX_Category_ParentCategoryId")
                .ForeignKey("FK_Category_ParentCategory", Schemas.Library, Tables.Category, Columns.Id)
                .OnDelete(System.Data.Rule.None);
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_Category_ParentCategory").OnTable(Tables.Category).InSchema(Schemas.Library);
        Delete.Column("ParentCategoryId").FromTable(Tables.Category).InSchema(Schemas.Library);
    }
}
