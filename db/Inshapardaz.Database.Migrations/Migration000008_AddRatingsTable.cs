using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000008)]
public class Migration000008_AddRatingsTable : Migration
{
    public override void Up()
    {
        Create.Table(Tables.BookRatings).InSchema(Schemas.Library)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn(Columns.BookId).AsInt32().NotNullable()
                .ForeignKey("FK_BookRatings_Book", Schemas.Library, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("LibraryId").AsInt32().NotNullable()
                .ForeignKey("FK_BookRatings_Library", Schemas.Library, Tables.Library, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("AccountId").AsInt32().NotNullable()
                .ForeignKey("FK_BookRatings_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
            // 1-5 star rating, enforced at the application layer (same approach as other
            // bounded/enum-like values in this schema, e.g. Book.Status).
            .WithColumn("Value").AsInt32().NotNullable()
            .WithColumn("DateAdded").AsDateTime2().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("DateUpdated").AsDateTime2().Nullable();

        // One rating per account per book - upserted in place rather than keyed by a
        // client-generated id, unlike Bookmarks/Notes which are a collection per book.
        Create.UniqueConstraint("UNQ_BookRatings_Book_Account")
            .OnTable(Tables.BookRatings).WithSchema(Schemas.Library)
            .Columns(Columns.BookId, "AccountId");

        Create.Table(Tables.IssueRatings).InSchema(Schemas.Library)
            .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
            .WithColumn("IssueId").AsInt32().NotNullable()
                .ForeignKey("FK_IssueRatings_Issue", Schemas.Library, Tables.Issue, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("LibraryId").AsInt32().NotNullable()
                .ForeignKey("FK_IssueRatings_Library", Schemas.Library, Tables.Library, Columns.Id).OnDelete(System.Data.Rule.Cascade)
            .WithColumn("AccountId").AsInt32().NotNullable()
                .ForeignKey("FK_IssueRatings_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
            .WithColumn("Value").AsInt32().NotNullable()
            .WithColumn("DateAdded").AsDateTime2().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("DateUpdated").AsDateTime2().Nullable();

        Create.UniqueConstraint("UNQ_IssueRatings_Issue_Account")
            .OnTable(Tables.IssueRatings).WithSchema(Schemas.Library)
            .Columns("IssueId", "AccountId");
    }

    public override void Down()
    {
        Delete.Table(Tables.IssueRatings).InSchema(Schemas.Library);
        Delete.Table(Tables.BookRatings).InSchema(Schemas.Library);
    }
}
