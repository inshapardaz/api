using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

// Meaning dictionaries (imported from StarDict/DSL or built by hand) and their articles. Every
// dictionary is public to read; OwnerId is the account allowed to change it.
[Migration(000014)]
public class Migration000014_AddDictionaries : Migration
{
    public override void Up()
    {
        Create.Table(Tables.Dictionaries).InSchema(Schemas.Language)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn(Columns.Name).AsString(500).NotNullable()
            .WithColumn(Columns.Description).AsString(2000).Nullable()
            .WithColumn("Format").AsString(20).NotNullable()
            .WithColumn("SourceLanguage").AsString(100).Nullable()
            .WithColumn("TargetLanguage").AsString(100).Nullable()
            .WithColumn("EntryCount").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("OwnerId").AsInt32().Nullable()
            .WithColumn("OriginalFileName").AsString(500).Nullable()
            .WithColumn(Columns.DateCreated).AsDateTime().NotNullable();

        Create.ForeignKey("FK_Dictionaries_Accounts")
            .FromTable(Tables.Dictionaries).InSchema(Schemas.Language).ForeignColumn("OwnerId")
            .ToTable(Tables.Accounts).InSchema(Schemas.Dbo).PrimaryColumn(Columns.Id)
            .OnDelete(System.Data.Rule.SetNull);

        Create.Table(Tables.DictionaryArticles).InSchema(Schemas.Language)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn("DictionaryId").AsInt64().NotNullable()
            .WithColumn("Headword").AsString(500).NotNullable()
            .WithColumn("HeadwordNormalised").AsString(500).NotNullable()
            .WithColumn("Pronunciation").AsString(500).Nullable()
            .WithColumn("Senses").AsString(int.MaxValue).Nullable()
            .WithColumn("Links").AsString(int.MaxValue).Nullable()
            .WithColumn("RawDefinition").AsString(int.MaxValue).Nullable()
            .WithColumn("Alternates").AsString(int.MaxValue).Nullable();

        Create.ForeignKey("FK_DictionaryArticles_Dictionaries")
            .FromTable(Tables.DictionaryArticles).InSchema(Schemas.Language).ForeignColumn("DictionaryId")
            .ToTable(Tables.Dictionaries).InSchema(Schemas.Language).PrimaryColumn(Columns.Id)
            .OnDelete(System.Data.Rule.Cascade);

        Create.Index("IX_DictionaryArticles_Dictionary_Headword")
            .OnTable(Tables.DictionaryArticles).InSchema(Schemas.Language)
            .OnColumn("DictionaryId").Ascending()
            .OnColumn("HeadwordNormalised").Ascending();

        Create.Index("IX_DictionaryArticles_Headword")
            .OnTable(Tables.DictionaryArticles).InSchema(Schemas.Language)
            .OnColumn("HeadwordNormalised").Ascending();
    }

    public override void Down()
    {
        Delete.Table(Tables.DictionaryArticles).InSchema(Schemas.Language);
        Delete.Table(Tables.Dictionaries).InSchema(Schemas.Language);
    }
}
