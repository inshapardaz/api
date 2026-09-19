using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000010)]
public class Migration000010_AddAccountDeletedFlag : Migration
{
    public override void Up()
    {
        Alter.Table(Tables.Accounts).InSchema(Schemas.Dbo)
            .AddColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        Delete.Column("IsDeleted").FromTable(Tables.Accounts).InSchema(Schemas.Dbo);
    }
}
