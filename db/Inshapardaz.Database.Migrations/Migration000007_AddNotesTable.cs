using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000007)]
public class Migration000007_AddNotesTable : Migration
{
    public override void Up()
    {
        Create.Table(Tables.Notes).InSchema(Schemas.Library)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn(Columns.BookId).AsInt32().NotNullable()
                .ForeignKey("FK_Notes_Book", Schemas.Library, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("LibraryId").AsInt32().NotNullable()
                .ForeignKey("FK_Notes_Library", Schemas.Library, Tables.Library, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("AccountId").AsInt32().NotNullable()
                .ForeignKey("FK_Notes_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
            // The client's own note id (e.g. a UUID it generated) - not this table's own Id - so a
            // client can PUT/DELETE idempotently by an id it minted itself and safely retry a
            // create without risking a duplicate row. See the unique constraint below - same
            // reasoning as Bookmarks.ClientId (Migration000006).
            .WithColumn("ClientId").AsString(64).NotNullable()
            // Opaque, client-defined chapter/anchor identifier - same idea as Bookmarks.ChapterId.
            .WithColumn("ChapterId").AsString(128).NotNullable()
            // The highlighted range's character offsets within the chapter.
            .WithColumn("StartOffset").AsInt32().NotNullable()
            .WithColumn("EndOffset").AsInt32().NotNullable()
            // The highlighted excerpt itself, so the note still renders sensibly even if the
            // underlying content is re-typed/edited later and the offsets drift.
            .WithColumn("Text").AsString(int.MaxValue).NotNullable()
            // The user's own annotation - nullable, since a highlight with no comment is still a
            // valid note.
            .WithColumn("Comment").AsString(int.MaxValue).Nullable()
            .WithColumn("DateAdded").AsDateTime2().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("DateUpdated").AsDateTime2().Nullable();

        Create.UniqueConstraint("UNQ_Notes_Book_Account_Client")
            .OnTable(Tables.Notes).WithSchema(Schemas.Library)
            .Columns(Columns.BookId, "AccountId", "ClientId");
    }

    public override void Down()
    {
        Delete.Table(Tables.Notes).InSchema(Schemas.Library);
    }
}
