using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

// Per-language transliteration dictionary: romanised spelling (lower-cased, up to three words) to
// the word's native spelling, used before the rule-based conversion.
[Migration(000015)]
public class Migration000015_AddTransliterationEntries : Migration
{
    public override void Up()
    {
        Create.Table(Tables.TransliterationEntries).InSchema(Schemas.Language)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn(Columns.Language).AsString(10).NotNullable()
            .WithColumn("Roman").AsString(200).NotNullable()
            .WithColumn("Native").AsString(200).NotNullable()
            .WithColumn("Meaning").AsString(500).Nullable()
            .WithColumn("Category").AsString(100).Nullable();

        Create.Index("UQ_TransliterationEntries_Language_Roman")
            .OnTable(Tables.TransliterationEntries).InSchema(Schemas.Language)
            .OnColumn(Columns.Language).Ascending()
            .OnColumn("Roman").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(Tables.TransliterationEntries).InSchema(Schemas.Language);
    }
}
