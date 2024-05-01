﻿using System.ComponentModel.Design;
using FluentMigrator;

namespace Inshapardaz.Database.Migrations
{
    [Migration(000100)]
    public class Migration000100_Consolidation : Migration
    {
        public override void Up()
        {
            // Create.Schema(Schemas.Dbo);

            //     Accounts
            //==========================================================================
            Create.Table(Tables.Accounts).
                InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey("PK_Accounts").Indexed().Identity()
                .WithColumn("Email").AsString().Nullable()
                .WithColumn("PasswordHash").AsString().Nullable()
                .WithColumn("AcceptTerms").AsBoolean().NotNullable()
                .WithColumn("VerificationToken").AsString().Nullable()
                .WithColumn("Verified").AsDateTime2().Nullable()
                .WithColumn("ResetToken").AsString().Nullable()
                .WithColumn("ResetTokenExpires").AsDateTime2().Nullable()
                .WithColumn("PasswordReset").AsDateTime2().Nullable()
                .WithColumn("Created").AsDateTime2().NotNullable()
                .WithColumn("Updated").AsDateTime2().Nullable()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("IsSuperAdmin").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("InvitationCode").AsString().Nullable()
                .WithColumn("InvitationCodeExpiry").AsDateTime2().Nullable();

            //--------------------------------------------------------------------------------

            Create.Table("RefreshToken")
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey("PK_RefreshToken").Indexed().Identity()
                .WithColumn("AccountId").AsInt32().NotNullable()
                    .ForeignKey("FK_RefreshToken_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("Token").AsString().Nullable()
                .WithColumn("Expires").AsDateTime2().NotNullable()
                .WithColumn("Created").AsDateTime2().NotNullable()
                .WithColumn("CreatedByIp").AsString().Nullable()
                .WithColumn("Revoked").AsDateTime2().Nullable()
                .WithColumn("RevokedByIp").AsString().Nullable()
                .WithColumn("ReplacedByToken").AsString().Nullable();


            //     File
            //==========================================================================

            Create.Table(Tables.File)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn(Columns.DateCreated).AsDateTime2()
                .WithColumn("FileName").AsString(int.MaxValue).Nullable()
                .WithColumn("MimeType").AsString(int.MaxValue).Nullable()
                .WithColumn("FilePath").AsString(int.MaxValue).Nullable()
                .WithColumn(Columns.IsPublic).AsBoolean().WithDefaultValue(0);

            Create.Table("FileData")
                .InSchema(Schemas.Dbo)
                .WithColumn("Path").AsString().PrimaryKey()
                .WithColumn("Content").AsBinary(int.MaxValue);

            //     Library
            //==========================================================================
            Create.Table(Tables.Library)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn(Columns.Name).AsString(256).NotNullable()
                .WithColumn(Columns.Language).AsString(10).NotNullable().WithDefaultValue("en")
                .WithColumn("SupportsPeriodicals").AsBoolean().WithDefaultValue(false)
                .WithColumn("PrimaryColor").AsString().Nullable()
                .WithColumn("SecondaryColor").AsString().Nullable()
                .WithColumn("OwnerEmail").AsString().Nullable()
                .WithColumn("Public").AsBoolean().WithDefaultValue(0)
                .WithColumn("DatabaseConnection").AsString().Nullable()
                .WithColumn("FileStoreType").AsString().WithDefaultValue("Database")
                .WithColumn("FileStoreSource").AsString().Nullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn(Columns.ImageId).AsInt64().Nullable()
                    .ForeignKey("FK_Library_File", Schemas.Dbo, Tables.File, Columns.Id);

            Create.Table("AccountLibrary")
                .WithColumn("AccountId").AsInt32()
                    .ForeignKey("FK_AccountLibrary_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_AccountLibrary_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("Role").AsInt32();

            //     Categories
            //==========================================================================
            Create.Table(Tables.Category)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn(Columns.Name).AsString(int.MaxValue).NotNullable()
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_Category_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade);


            //     Series
            //==========================================================================
            Create.Table(Tables.Series)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn(Columns.Name).AsString().NotNullable()
                .WithColumn(Columns.Description).AsString(int.MaxValue).Nullable()
                .WithColumn(Columns.ImageId).AsInt64().Nullable()
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_Series_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade);

            Create.Table(Tables.SeriesCategory)
                .InSchema(Schemas.Dbo)
                .WithColumn("SeriesId").AsInt32()
                    .ForeignKey("FK_SeriesCategory_SeriesId", Schemas.Dbo, Tables.Series, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("CategoryId").AsInt32().Indexed("IX_SeriesCategory_CategoryId")
                    .ForeignKey("FK_SeriesCategory_CategoryId", Schemas.Dbo, Tables.Category, Columns.Id);

            Create.PrimaryKey("PK_SeriesCategory")
                .OnTable(Tables.SeriesCategory).WithSchema(Schemas.Dbo)
                .Columns("SeriesId", "CategoryId");

            //     Author
            //==========================================================================

            Create.Table(Tables.Author)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn(Columns.Name).AsString().NotNullable().Indexed("IDX_AuthorName")
                .WithColumn(Columns.ImageId).AsInt64().Nullable()
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_Author_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("AuthorType").AsInt32().WithDefaultValue(0).Nullable();

            //     Book
            //==========================================================================
            Create.Table(Tables.Book)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn("Title").AsString(512).NotNullable().Indexed("IDX_BookTitle")
                .WithColumn(Columns.Description).AsString(int.MaxValue).Nullable()
                .WithColumn(Columns.ImageId).AsInt64().Nullable()
                    .ForeignKey("FK_Book_Image", Schemas.Dbo, Tables.File, Columns.Id)
                .WithColumn(Columns.IsPublic).AsBoolean().WithDefaultValue(false).Indexed("IDX_PublicBook")
                .WithColumn("IsPublished").AsBoolean().WithDefaultValue(false)
                .WithColumn(Columns.Language).AsString(10)
                .WithColumn("Status").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("SeriesId").AsInt32().Nullable().Indexed("IX_Book_SeriesId")
                    .ForeignKey("FK_Book_Series", Schemas.Dbo, Tables.Series, Columns.Id)
                .WithColumn("SeriesIndex").AsInt32().Nullable()
                .WithColumn("Copyrights").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("YearPublished").AsInt32().Nullable()
                .WithColumn("DateAdded").AsDateTime2().NotNullable().WithDefaultValue("0001-01-01T00:00:00.000")
                .WithColumn("DateUpdated").AsDateTime2().NotNullable().WithDefaultValue("0001-01-01T00:00:00.000")
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_Book_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("Source").AsString().Indexed("IDX_Book_Source").Nullable()
                .WithColumn("Publisher").AsString().Indexed("IDX_Book_Publisher").Nullable();
            //--------------------------------------------------------------------------------

            Create.Table("BookAuthor").InSchema(Schemas.Dbo)
                .WithColumn("BookId").AsInt32().Indexed("IX_BookAuthor_BookId")
                    .ForeignKey("FK_BookAuthor_Book", Schemas.Dbo, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("AuthorId").AsInt32().Indexed("IX_BookAuthor_AuthorId")
                    .ForeignKey("FK_BookAuthor_Author", Schemas.Dbo, Tables.Author, Columns.Id);

            Create.PrimaryKey("PK_BookAuthor")
                .OnTable("BookAuthor").WithSchema(Schemas.Dbo)
                .Columns("BookId", "AuthorId");
            //--------------------------------------------------------------------------------

            Create.Table("BookContent")
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn(Columns.BookId).AsInt32().Indexed("IDX_BookContent_Book")
                    .ForeignKey("FK_BookContent_Book", Schemas.Dbo, Tables.Book, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn(Columns.FileId).AsInt64()
                    .ForeignKey("FK_BookContent_File", Schemas.Dbo, Tables.File, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn(Columns.Language).AsString(10).Nullable();

            //--------------------------------------------------------------------------------

            Create.Table(Tables.BookCategory)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.BookId).AsInt32()
                    .ForeignKey("FK_BookCategory_Book_BookId", Schemas.Dbo, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("CategoryId").AsInt32().Indexed("IX_BookCategory_CategoryId")
                    .ForeignKey("FK_BookCategory_Category", Schemas.Dbo, Tables.Category, Columns.Id);
            ;

            Create.PrimaryKey("PK_BookCategory")
                .OnTable(Tables.BookCategory).WithSchema(Schemas.Dbo)
                .Columns(Columns.BookId, "CategoryId");
            //--------------------------------------------------------------------------------

            Create.Table(Tables.Chapter)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn("Title").AsString(int.MaxValue).NotNullable()
                .WithColumn(Columns.BookId).AsInt32().Indexed("IX_Chapter_BookId")
                    .ForeignKey("FK_Chapter_Book", Schemas.Dbo, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("ChapterNumber").AsInt32()
                .WithColumn("Status").AsInt32().WithDefaultValue(0)
                .WithColumn("WriterAccountId").AsInt32().Nullable()
                    .ForeignKey("FK_Chapter_Writer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("WriterAssignTimeStamp").AsDateTime2().Nullable()
                .WithColumn("ReviewerAccountId").AsInt32().Nullable()
                    .ForeignKey("FK_Chapter_Reviewer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("ReviewerAssignTimeStamp").AsDateTime2().Nullable();
            //--------------------------------------------------------------------------------

            Create.Table(Tables.ChapterContent)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn("ChapterId").AsInt64().Indexed("IX_ChapterContent_ChapterId")
                    .ForeignKey("FK_ChapterContent_Chapter", Schemas.Dbo, Tables.Chapter, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Language").AsString().Nullable()
                .WithColumn("Text").AsString(int.MaxValue).Nullable();

            Create.UniqueConstraint("UQ_ChapterId_Language")
                  .OnTable(Tables.ChapterContent).Columns("ChapterId", "Language");
            //--------------------------------------------------------------------------------

            Create.Table(Tables.BookPage)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn(Columns.BookId).AsInt32().NotNullable().Indexed("IX_BookPage_BookId")
                    .ForeignKey("FK_BookPage_Book", Schemas.Dbo, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Text").AsString(int.MaxValue).Nullable()
                .WithColumn("SequenceNumber").AsInt32().Nullable()
                .WithColumn("ImageId").AsInt64().Nullable()
                .WithColumn("Status").AsInt32().WithDefaultValue(0)
                .WithColumn("WriterAccountId").AsInt32().Nullable()
                    .ForeignKey("FK_BookPage_Writer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("WriterAssignTimeStamp").AsDateTime2().Nullable()
                .WithColumn("ReviewerAccountId").AsInt32().Nullable()
                    .ForeignKey("FK_BookPage_Reviewer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("ReviewerAssignTimeStamp").AsDateTime2().Nullable()
                .WithColumn("ChapterId").AsInt64().Nullable()
                    .ForeignKey("FK_BookPage_Chapter", Schemas.Dbo, Tables.Chapter, Columns.Id);
            //--------------------------------------------------------------------------------

            Create.Table(Tables.BookShelf).InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_BookShelf_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("AccountId").AsInt32().NotNullable()
                    .ForeignKey("FK_BookShelf_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn(Columns.Name).AsString()
                .WithColumn(Columns.Description).AsString().Nullable()
                .WithColumn(Columns.IsPublic).AsBoolean().Nullable()
                .WithColumn(Columns.ImageId).AsInt64().Nullable();
            //--------------------------------------------------------------------------------

            Create.Table(Tables.BookShelfBook).InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn(Columns.BookId).AsInt32().NotNullable()
                    .ForeignKey("FK_BookShelfBook_Book", Schemas.Dbo, Tables.Book, Columns.Id)
                .WithColumn("BookShelfId").AsInt32().NotNullable()
                    .ForeignKey("FK_BookShelfBook_BookShelf", Schemas.Dbo, Tables.BookShelf, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Index").AsInt32().Nullable();
            //--------------------------------------------------------------------------------

            Create.Table(Tables.FavoriteBooks)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.BookId).AsInt32().Indexed("IX_FavoriteBooks_BookId")
                    .ForeignKey("FK_FavoriteBooks_Book", Schemas.Dbo, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_FavoriteBooks_Library", Schemas.Dbo, Tables.Library, Columns.Id)
                .WithColumn("AccountId").AsInt32()
                    .ForeignKey("FK_FavoriteBooks_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("DateAdded").AsDateTime2();

            Create.PrimaryKey("PK_FavoriteBooks")
               .OnTable(Tables.FavoriteBooks).WithSchema(Schemas.Dbo)
               .Columns(Columns.BookId, "AccountId");
            //--------------------------------------------------------------------------------

            Create.Table(Tables.RecentBooks)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.BookId).AsInt32().Indexed("IX_RecentBooks_BookId")
                    .ForeignKey("FK_RecentBooks_Book", Schemas.Dbo, Tables.Book, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("LibraryId").AsInt32().Nullable()
                    .ForeignKey("FK_RecentBooks_Library", Schemas.Dbo, Tables.Library, Columns.Id)
                .WithColumn("AccountId").AsInt32()
                    .ForeignKey("FK_RecentBooks_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("DateRead").AsDateTime2();

            Create.PrimaryKey("PK_RecentBooks")
                .OnTable(Tables.RecentBooks).WithSchema(Schemas.Dbo)
                .Columns(Columns.BookId, "AccountId");
            //     Article
            //==========================================================================
            Create.Table(Tables.Article).InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn("Title").AsString(512).NotNullable()
                .WithColumn("Status").AsInt32().WithDefaultValue(0)
                .WithColumn(Columns.IsPublic).AsBoolean().WithDefaultValue(0)
                .WithColumn("Type").AsInt32().WithDefaultValue(0)
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_Article_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("ImageId").AsInt64().ForeignKey("File", Columns.Id).Nullable()
                .WithColumn("WriterAccountId").AsInt32().Nullable()
                    .ForeignKey("FK_Article_Writer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("WriterAssignTimeStamp").AsDateTime2().Nullable()
                .WithColumn("ReviewerAccountId").AsInt32().Nullable()
                    .ForeignKey("FK_Articles_Reader_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("ReviewerAssignTimeStamp").AsDateTime2().Nullable()
                .WithColumn("SourceType").AsString().Nullable()
                .WithColumn("SourceId").AsInt32().Nullable()
                .WithColumn("LastModified").AsDateTime2();
            //--------------------------------------------------------------------------------

            Create.Table("ArticleAuthor").InSchema(Schemas.Dbo)
                .WithColumn("ArticleId").AsInt64()
                    .ForeignKey("FK_ArticleAuthor_Article", Schemas.Dbo, Tables.Article, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("AuthorId").AsInt32()
                    .ForeignKey("FK_ArticleAuthor_Author", Schemas.Dbo, Tables.Author, Columns.Id);

            Create.PrimaryKey("PK_ArticleAuthor")
                .OnTable("ArticleAuthor").WithSchema(Schemas.Dbo)
                .Columns("ArticleId", "AuthorId");
            //--------------------------------------------------------------------------------

            Create.Table("ArticleCategory").InSchema(Schemas.Dbo)
                .WithColumn("ArticleId").AsInt64()
                    .ForeignKey("FK_ArticleCategory_Article", Schemas.Dbo, Tables.Article, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("CategoryId").AsInt32()
                    .ForeignKey("FK_ArticleCategory_Category", Schemas.Dbo, Tables.Category, Columns.Id);

            Create.PrimaryKey("PK_ArticleCategory")
                .OnTable("ArticleCategory").WithSchema(Schemas.Dbo)
                .Columns("ArticleId", "CategoryId");
            //--------------------------------------------------------------------------------

            Create.Table("ArticleContent").InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn("ArticleId").AsInt64().Indexed("IX_ArticleContent_ArticleId")
                    .ForeignKey("FK_ArticleContent_Article", Schemas.Dbo, Tables.Article, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn(Columns.Language).AsString().Nullable()
                .WithColumn("Text").AsString(int.MaxValue).NotNullable()
                .WithColumn("Layout").AsString().Nullable();
            //--------------------------------------------------------------------------------

            Create.Table("ArticleFavorite").InSchema(Schemas.Dbo)
                .WithColumn("ArticleId").AsInt64()
                    .ForeignKey("FK_ArticleFavorite_Article", Tables.Article, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("AccountId").AsInt32()
                    .ForeignKey("FK_ArticleFavorite_Account", Tables.Accounts, Columns.Id)
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_ArticleFavorite_Library", Schemas.Dbo, Tables.Library, Columns.Id);

            Create.PrimaryKey("PK_ArticleFavorite")
                .OnTable("ArticleFavorite").WithSchema(Schemas.Dbo)
                .Columns("ArticleId", "AccountId");
            //--------------------------------------------------------------------------------

            Create.Table("ArticleRead").InSchema(Schemas.Dbo)
                .WithColumn("ArticleId").AsInt64()
                    .ForeignKey("FK_ArticleRead_Article", Tables.Article, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("AccountId").AsInt32()
                    .ForeignKey("FK_ArticleRead_Account", Tables.Accounts, Columns.Id)
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_ArticleRead_Library", Schemas.Dbo, Tables.Library, Columns.Id)
                .WithColumn("DateRead").AsDateTime2();

            Create.PrimaryKey("PK_ArticleRead")
                            .OnTable("ArticleRead").WithSchema(Schemas.Dbo)
                            .Columns("ArticleId", "AccountId");
            //  Periodical
            //==========================================================================

            Create.Table(Tables.Periodical)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn(Columns.Description).AsString(int.MaxValue).Nullable()
                .WithColumn(Columns.ImageId).AsInt64().Nullable()
                .WithColumn("LibraryId").AsInt32()
                    .ForeignKey("FK_Periodical_Library", Schemas.Dbo, Tables.Library, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn(Columns.Language).AsString(10).Nullable()
                .WithColumn("Frequency").AsInt32().WithDefaultValue(0);
            //--------------------------------------------------------------------------------

            Create.Table(Tables.PeriodicalCategory)
                .InSchema(Schemas.Dbo)
                .WithColumn("PeriodicalId").AsInt32().NotNullable()
                    .ForeignKey("FK_PeriodicalCategory_Periodical", Schemas.Dbo, Tables.Periodical, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                .WithColumn("CategoryId").AsInt32().NotNullable()
                    .ForeignKey("FK_PeriodicalCategory_Category_CategoryId", Schemas.Dbo, Tables.Category, Columns.Id);

            Create.PrimaryKey("PK_PeriodicalCategory")
                .OnTable(Tables.PeriodicalCategory).WithSchema(Schemas.Dbo)
                .Columns("PeriodicalId", "CategoryId");
            //--------------------------------------------------------------------------------

            Create.Table(Tables.Issue)
                    .InSchema(Schemas.Dbo)
                    .WithColumn(Columns.Id).AsInt32().PrimaryKey().Identity()
                    .WithColumn("PeriodicalId").AsInt32().NotNullable().Indexed("IX_Issue_PeriodicalId")
                        .ForeignKey("FK_Issue_Periodical_PeriodicalId", Schemas.Dbo, Tables.Periodical, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                    .WithColumn("VolumeNumber").AsInt32().NotNullable()
                    .WithColumn("IssueNumber").AsInt32().NotNullable()
                    .WithColumn(Columns.ImageId).AsInt64().Nullable()
                    .WithColumn("IssueDate").AsDateTime2().NotNullable()
                    .WithColumn("IsPublic").AsBoolean().WithDefaultValue(true);

            Create.UniqueConstraint("UNQ_VOLUME_ISSUE")
                .OnTable(Tables.Issue).WithSchema(Schemas.Dbo)
                .Columns("PeriodicalId", "VolumeNumber", "IssueNumber");
            //--------------------------------------------------------------------------------

            Create.Table("IssueArticle")
                    .InSchema(Schemas.Dbo)
                    .WithColumn(Columns.Id).AsInt64().PrimaryKey("PK_IssueArticle").Identity()
                    .WithColumn("Title").AsString().NotNullable()
                    .WithColumn("IssueId").AsInt32().NotNullable()
                        .ForeignKey("FK_IssueArticle_IssueId", Schemas.Dbo, Tables.Issue, Columns.Id).OnDeleteOrUpdate(System.Data.Rule.Cascade)
                    .WithColumn("Status").AsInt32().WithDefaultValue(0)
                    .WithColumn("WriterAccountId").AsInt32().Nullable()
                        .ForeignKey("FK_IssueArticle_Writer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                    .WithColumn("WriterAssignTimeStamp").AsDateTime2().Nullable()
                    .WithColumn("ReviewerAccountId").AsInt32().Nullable()
                        .ForeignKey("FK_IssueArticle_Reviewer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                    .WithColumn("ReviewerAssignTimeStamp").AsDateTime2().Nullable()
                    .WithColumn("SequenceNumber").AsInt32().Nullable()
                    .WithColumn("SeriesName").AsString().Nullable().Indexed("IX_IssueArticle_SeriesId")
                    .WithColumn("SeriesIndex").AsInt32().Nullable();

            //--------------------------------------------------------------------------------

            Create.Table("IssueArticleAuthor")
                .WithColumn("IssueArticleId").AsInt64().NotNullable()
                    .ForeignKey("FK_IssueArticleAuthor_IssueArticle", Schemas.Dbo, "IssueArticle", Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("AuthorId").AsInt32().NotNullable()
                    .ForeignKey("FK_IssueArticleAuthor_Author", Schemas.Dbo, Tables.Author, Columns.Id);

            Create.PrimaryKey("PK_IssueArticleAuthor")
                .OnTable("IssueArticleAuthor").WithSchema(Schemas.Dbo)
                .Columns("IssueArticleId", "AuthorId");
            //--------------------------------------------------------------------------------

            Create.Table("IssueContent")
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn("IssueId").AsInt32().Indexed("IX_IssueContent_IssueId")
                    .ForeignKey("FK_IssueContent_Issue", Schemas.Dbo, Tables.Issue, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn(Columns.Language).AsString().Nullable()
                .WithColumn("MimeType").AsString().Nullable()
                .WithColumn(Columns.FileId).AsInt64().Indexed("IX_IssueContent_FileId")
                    .ForeignKey("FK_IssueContent_File", Schemas.Dbo, Tables.File, Columns.Id).OnDelete(System.Data.Rule.Cascade);

            //--------------------------------------------------------------------------------

            Create.Table(Tables.IssuePage)
                .InSchema(Schemas.Dbo)
                .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn("IssueId").AsInt32().NotNullable().Indexed("IX_IssuePage_IssueId")
                    .ForeignKey("FK_IssuePage_Issue", Schemas.Dbo, Tables.Issue, Columns.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Text").AsString(int.MaxValue).Nullable()
                .WithColumn("ArticleId").AsInt64().Nullable()
                    .ForeignKey("FK_IssuePage_IssueArticle", Schemas.Dbo, "IssueArticle", Columns.Id)
                .WithColumn("SequenceNumber").AsInt32().Nullable()
                .WithColumn("ImageId").AsInt64().Nullable()
                    .ForeignKey("FK_IssuePage_File", Schemas.Dbo, Tables.File, Columns.Id)
                .WithColumn("Status").AsInt32().WithDefaultValue(0)
                .WithColumn("WriterAccountId").AsInt32().Nullable()
                        .ForeignKey("FK_IssuePage_Writer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("WriterAssignTimeStamp").AsDateTime2().Nullable()
                .WithColumn("ReviewerAccountId").AsInt32().Nullable()
                        .ForeignKey("FK_IssuePage_Reviewer_Accounts", Schemas.Dbo, Tables.Accounts, Columns.Id)
                .WithColumn("ReviewerAssignTimeStamp").AsDateTime2().Nullable();
            //--------------------------------------------------------------------------------

            Create.Table("IssueArticleContent").InSchema(Schemas.Dbo)
                    .WithColumn(Columns.Id).AsInt64().PrimaryKey().Identity()
                    .WithColumn("Text").AsString(int.MaxValue).Nullable()
                    .WithColumn("ArticleId").AsInt64().Nullable()
                        .ForeignKey("FK_IssueArticleContent_IssueArticle", Schemas.Dbo, "IssueArticle", Columns.Id).OnDelete(System.Data.Rule.Cascade)
                    .WithColumn(Columns.Language).AsString(10).Nullable();

            Create.UniqueConstraint("UQ_IssueArticleId_Language")
                .OnTable("IssueArticleContent").WithSchema(Schemas.Dbo)
                .Columns("ArticleId", "Language");

            // Language
            //==========================================================================
            Create.Table(Tables.Corrections).InSchema(Schemas.Dbo)
                    .WithColumn(Columns.Id).AsInt64().Identity().PrimaryKey()
                    .WithColumn("Language").AsString().Indexed("IDX_CORRECTION_LANGUAGE")
                    .WithColumn("Profile").AsString().Indexed("IDX_PROFILE")
                    .WithColumn("IncorrectText").AsString().NotNullable()
                    .WithColumn("CorrectText").AsString().NotNullable()
                    .WithColumn("Usage").AsInt64().WithDefaultValue(0)
                    .WithColumn("CompleteWord").AsBoolean().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Table(Tables.Corrections).InSchema(Schemas.Dbo);
            Delete.Table("IssueArticleContent").InSchema(Schemas.Dbo);
            Delete.Table(Tables.IssuePage).InSchema(Schemas.Dbo);
            Delete.Table("IssueContent").InSchema(Schemas.Dbo);
            Delete.Table("IssueArticleAuthor").InSchema(Schemas.Dbo);
            Delete.Table("IssueArticle").InSchema(Schemas.Dbo);
            Delete.Table(Tables.Issue).InSchema(Schemas.Dbo);
            Delete.Table(Tables.PeriodicalCategory).InSchema(Schemas.Dbo);
            Delete.Table(Tables.Periodical).InSchema(Schemas.Dbo);

            Delete.Table("ArticleAuthor").InSchema(Schemas.Dbo);
            Delete.Table("ArticleCategory").InSchema(Schemas.Dbo);
            Delete.Table("ArticleContent").InSchema(Schemas.Dbo);
            Delete.Table("ArticleFavorite").InSchema(Schemas.Dbo);
            Delete.Table("ArticleRead").InSchema(Schemas.Dbo);
            Delete.Table(Tables.Article).InSchema(Schemas.Dbo);

            Delete.Table(Tables.BookShelfBook).InSchema(Schemas.Dbo);
            Delete.Table(Tables.BookShelf).InSchema(Schemas.Dbo);
            Delete.Table(Tables.FavoriteBooks).InSchema(Schemas.Dbo);
            Delete.Table(Tables.RecentBooks).InSchema(Schemas.Dbo);
            Delete.Table(Tables.BookPage).InSchema(Schemas.Dbo);
            Delete.Table(Tables.ChapterContent).InSchema(Schemas.Dbo);
            Delete.Table(Tables.Chapter).InSchema(Schemas.Dbo);
            Delete.Table(Tables.BookCategory).InSchema(Schemas.Dbo);
            Delete.Table("BookContent").InSchema(Schemas.Dbo);
            Delete.Table("BookAuthor").InSchema(Schemas.Dbo);
            Delete.Table(Tables.Book).InSchema(Schemas.Dbo);

            Delete.Table(Tables.SeriesCategory).InSchema(Schemas.Dbo);
            Delete.Table(Tables.Series).InSchema(Schemas.Dbo);
            Delete.Table(Tables.Category).InSchema(Schemas.Dbo);
            Delete.Table(Tables.Author).InSchema(Schemas.Dbo);
            Delete.Table("AccountLibrary").InSchema(Schemas.Dbo);
            Delete.Table(Tables.Library).InSchema(Schemas.Dbo);
            Delete.Table("FileData").InSchema(Schemas.Dbo);
            Delete.Table(Tables.File).InSchema(Schemas.Dbo);
            Delete.Table("RefreshToken").InSchema(Schemas.Dbo);
            Delete.Table(Tables.Accounts).InSchema(Schemas.Dbo);
        }
    }
}
