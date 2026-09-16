using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000006)]
public class Migration000006_AddBookmarksTable : Migration
{
    public override void Up()
    {
        Create.Table(Tables.Bookmarks).InSchema(Schemas.Library)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn(Columns.BookId).AsInt32().NotNullable()
                .ForeignKey("FK_Bookmarks_Book", Schemas.Library, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("LibraryId").AsInt32().NotNullable()
                .ForeignKey("FK_Bookmarks_Library", Schemas.Library, Tables.Library, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("AccountId").AsInt32().NotNullable()
                .ForeignKey("FK_Bookmarks_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
            // The client's own bookmark id (e.g. a UUID it generated) - not this table's own Id -
            // so a client can PUT/DELETE idempotently by an id it minted itself and safely retry a
            // create without risking a duplicate row. See the unique constraint below.
            .WithColumn("ClientId").AsString(64).NotNullable()
            // Opaque, client-defined chapter/anchor identifier - same idea as the anchor a reader
            // already sends via ReadProgressView.ProgressId.
            .WithColumn("ChapterId").AsString(128).NotNullable()
            // Within-chapter offset, same unit/meaning as ReadProgressView.ProgressValue.
            .WithColumn("Position").AsDouble().NotNullable()
            .WithColumn(Columns.Name).AsString(256).NotNullable()
            .WithColumn("DateAdded").AsDateTime2().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("DateUpdated").AsDateTime2().Nullable();

        Create.UniqueConstraint("UNQ_Bookmarks_Book_Account_Client")
            .OnTable(Tables.Bookmarks).WithSchema(Schemas.Library)
            .Columns(Columns.BookId, "AccountId", "ClientId");
    }

    public override void Down()
    {
        Delete.Table(Tables.Bookmarks).InSchema(Schemas.Library);
    }
}
