using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

// Which replacement writers chose for a misspelt word, and how often; used to rank spell-check
// suggestions. SourceWord holds the normalised form of the misspelt word.
[Migration(000013)]
public class Migration000013_AddSpellingReplacements : Migration
{
    public override void Up()
    {
        Create.Table(Tables.SpellingReplacements).InSchema(Schemas.Language)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn(Columns.Language).AsString(10).NotNullable()
            .WithColumn("SourceWord").AsString(256).NotNullable()
            .WithColumn("Replacement").AsString(256).NotNullable()
            .WithColumn("Count").AsInt32().NotNullable().WithDefaultValue(1)
            .WithColumn("LastUsedAt").AsDateTime().NotNullable();

        Create.Index("UQ_SpellingReplacements_Language_Source_Replacement")
            .OnTable(Tables.SpellingReplacements).InSchema(Schemas.Language)
            .OnColumn(Columns.Language).Ascending()
            .OnColumn("SourceWord").Ascending()
            .OnColumn("Replacement").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(Tables.SpellingReplacements).InSchema(Schemas.Language);
    }
}
