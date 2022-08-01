using Dapper;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class IssueDataHelper
    {
        public static void AddIssue(this IDbConnection connection, IssueDto issue)
        {
            var sql = @"Insert Into Issue (PeriodicalId, Volumenumber, IssueNumber, ImageId, IssueDate, IsPublic)
                        Output Inserted.Id
                        Values (@PeriodicalId, @Volumenumber, @IssueNumber, @ImageId, @IssueDate, @IsPublic)";
            var id = connection.ExecuteScalar<int>(sql, issue);
            issue.Id = id;
        }

        public static void AddIssues(this IDbConnection connection, IEnumerable<IssueDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssue(connection, issue);
            }
        }

        /*public static void AddBookToFavorites(this IDbConnection connection, int libraryId, int bookId, int accountId)
        {
            var sql = @"Insert into FavoriteBooks (LibraryId, BookId, AccountId, DateAdded)
                        Values (@LibraryId, @BookId, @AccountId, GETDATE())";
            connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, bookId, accountId });
        }

        public static void AddBooksToFavorites(this IDbConnection connection, int libraryId, IEnumerable<int> bookIds, int accountId)
        {
            bookIds.ForEach(id => connection.AddBookToFavorites(libraryId, id, accountId));
        }

        public static void AddBooksToRecentReads(this IDbConnection connection, int libraryId, IEnumerable<int> bookIds, int accountId)
        {
            bookIds.ForEach(id => connection.AddBookToRecentReads(libraryId, id, accountId));
        }

        public static void AddBookToRecentReads(this IDbConnection connection, int libraryId, int bookId, int accountId, DateTime? timestamp = null)
        {
            var sql = @"Insert into RecentBooks (LibraryId, BookId, AccountId, DateRead)
                        Values (@LibraryId, @BookId, @AccountId, @DateRead)";
            connection.ExecuteScalar<int>(sql, new { LibraryId = libraryId, bookId, accountId, DateRead = timestamp ?? DateTime.Now });
        }

        public static void AddBookToRecentReads(this IDbConnection connection, RecentBookDto dto)
        {
            var sql = @"Insert into RecentBooks (LibraryId, BookId, AccountId, DateRead)
                        Values (@LibraryId, @BookId, @AccountId, @DateRead)";
            connection.ExecuteScalar<int>(sql, dto);
        }*/

        public static void AddIssueFiles(this IDbConnection connection, int issueId, IEnumerable<IssueContentDto> contentDto) =>
            contentDto.ForEach(f => connection.AddIssueFile(issueId, f));

        public static void AddIssueFile(this IDbConnection connection, int issueId, IssueContentDto contentDto)
        {
            var sql = @"Insert Into IssueContent (IssueId, FileId, Language, MimeType)
                        Output Inserted.Id
                        Values (@IssueId, @FileId, @Language, @MimeType)";
            var id = connection.ExecuteScalar<int>(sql, new { IssueId = issueId, FileId = contentDto.FileId, Language = contentDto.Language, MimeType = contentDto.MimeType });
            contentDto.Id = id;
        }

        /*public static int GetBookCountByAuthor(this IDbConnection connection, int id)
        {
            var sql = @"SELECT Count(*)
                                FROM Book b
                                INNER JOIN BookAuthor ba ON ba.BookId = b.Id
                                WHERE ba.AuthorId = @Id  AND b.Status = 0";
            return connection.ExecuteScalar<int>(sql, new { Id = id });
        }*/

        public static IssueDto GetIssueById(this IDbConnection connection, int issueId)
        {
            var sql = @"SELECT * FROM Issue WHERE Id = @Id";
            return connection.QuerySingleOrDefault<IssueDto>(sql, new { Id = issueId });
        }

        public static IEnumerable<IssueDto> GetIssuesByPeriodical(this IDbConnection connection, int periodicalId)
        {
            var sql = @"SELECT i.* FROM Issue i
                        INNER JOIN Periodical p on p.Id = i.PeriodicalId
                        WHERE i.PeriodicalId = @PeriodicalId";
            return connection.Query<IssueDto>(sql, new { PeriodicalId = periodicalId });
        }
        public static string GetIssueImageUrl(this IDbConnection connection, int issueId)
        {
            var sql = @"SELECT f.FilePath FROM [File] f
                        INNER JOIN Issue i ON f.Id = i.ImageId
                        WHERE i.Id = @Id";
            return connection.QuerySingleOrDefault<string>(sql, new { Id = issueId });
        }

        public static FileDto GetIssueImage(this IDbConnection connection, int issueId)
        {
            var sql = @"Select f.* from [File] f
                        INNER JOIN Issue i ON f.Id = i.ImageId
                        WHERE i.Id = @Id";
            return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = issueId });
        }

        public static void DeleteIssues(this IDbConnection connection, IEnumerable<IssueDto> issues)
        {
            var sql = "Delete From Issue Where Id IN @Ids";
            connection.Execute(sql, new { Ids = issues.Select(f => f.Id) });
        }

        public static IEnumerable<IssueContentDto> GetIssueContents(this IDbConnection connection, int issueId)
        {
            string sql = @"Select bc.*, f.MimeType From IssueContent ic
                           INNER Join Issue i ON i.Id = ic.IssueId
                           INNER Join [File] f ON f.Id = ic.FileId
                           Where t.Id = @IssueId";

            return connection.Query<IssueContentDto>(sql, new
            {
                IssueId = issueId
            });
        }

        public static IssueContentDto GeIssueContent(this IDbConnection connection, int issueId, string language, string mimetype)
        {
            string sql = @"Select * From IssueContent ic
                           INNER Join Issue i ON i.Id = ic.IssueId
                           INNER Join [File] f ON f.Id = ic.FileId
                           Where i.Id = @IssueId AND ic.Language = @Language AND f.MimeType = @MimeType";

            return connection.QuerySingleOrDefault<IssueContentDto>(sql, new
            {
                IssueId = issueId,
                Language = language,
                MimeType = mimetype
            });
        }

        public static string GetIssueContentPath(this IDbConnection connection, int issueId, string language, string mimetype)
        {
            string sql = @"Select f.FilePath From IssueContent ic
                           INNER Join Issue i ON i.Id = ic.IssueId
                           INNER Join [File] f ON f.Id = ic.FileId
                           Where i.Id = @IssueId AND ic.Language = @Language AND f.MimeType = @MimeType";

            return connection.QuerySingleOrDefault<string>(sql, new
            {
                IssueId = issueId,
                Language = language,
                MimeType = mimetype
            });
        }
    }
}
