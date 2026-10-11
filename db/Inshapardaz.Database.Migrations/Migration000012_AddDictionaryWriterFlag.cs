using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

// Account-wide flag (not a per-library role) for users who manage the language tools:
// spelling, meaning dictionaries, transliteration and the autocorrect/punctuation lists.
[Migration(000012)]
public class Migration000012_AddDictionaryWriterFlag : Migration
{
    public override void Up()
    {
        Alter.Table(Tables.Accounts).InSchema(Schemas.Dbo)
            .AddColumn("IsDictionaryWriter").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        Delete.Column("IsDictionaryWriter").FromTable(Tables.Accounts).InSchema(Schemas.Dbo);
    }
}
