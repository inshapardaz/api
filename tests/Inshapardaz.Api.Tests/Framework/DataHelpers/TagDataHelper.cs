using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface ITagTestRepository
    {
        void AddTag(TagDto tag);

        void AddTags(IEnumerable<TagDto> tags);

        void DeleteTags(IEnumerable<TagDto> tags);

        TagDto GetTagById(int libraryId, int id);

        bool DoesTagExists(int id);

        IEnumerable<TagDto> GetTags(params int[] ids);
        IEnumerable<TagDto> GetTagsByBook(int id);

        IEnumerable<TagDto> GetTagsByPeriodical(int id);

        IEnumerable<TagDto> GetTagsByArticle(long id);

        void AddBooksToTag(IEnumerable<BookDto> books, TagDto tag);

        void AddBookToTags(int bookId, IEnumerable<TagDto> tags);

        void AddPeriodicalToTag(IEnumerable<PeriodicalDto> periodicals, TagDto tag);
        void AddPeriodicalToTags(int periodicalId, IEnumerable<TagDto> tags);

        void AddIssueToTags(long issueId, IEnumerable<TagDto> tags);
        void AddArticleToTags(long articleId, IEnumerable<TagDto> tags);
        IEnumerable<TagDto> GetTagsByIssue(int viewId);
    }

    public class MySqlTagTestRepository : ITagTestRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public MySqlTagTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public void AddTag(TagDto tag)
        {
            using var connection = _connectionProvider.GetConnection();
            var id = connection.ExecuteScalar<int>("INSERT INTO Tag (`Name`, LibraryId) VALUES (@Name, @LibraryId); SELECT LAST_INSERT_ID();", tag);
            tag.Id = id;
        }

        public void AddTags(IEnumerable<TagDto> tags)
        {
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
        }

        public void DeleteTags(IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            var sql = "DELETE FROM Tag WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = tags.Select(a => a.Id) });
        }

        public TagDto GetTagById(int libraryId, int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QuerySingleOrDefault<TagDto>("SELECT * FROM Tag WHERE Id = @Id AND LibraryId = @LibraryId",
                new { Id = id, LibraryId = libraryId });
        }

        public bool DoesTagExists(int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.ExecuteScalar<bool>("SELECT COUNT(*) FROM Tag WHERE Id = @Id", new { Id = id });
        }

        public IEnumerable<TagDto> GetTags(params int[] ids)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>("SELECT * FROM Tag WHERE Id IN @TagIds",
                new { TagIds = ids });
        }
        
        public IEnumerable<TagDto> GetTagsByBook(int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT c.* FROM Tag c
                                INNER JOIN BookTag bc ON c.Id = bc.TagId
                                WHERE bc.BookId = @BookId ", new { BookId = id });
        }

        public IEnumerable<TagDto> GetTagsByPeriodical(int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT c.* From Tag c
                                INNER JOIN PeriodicalTag pc ON c.Id = pc.TagId
                                WHERE pc.PeriodicalId = @PeriodicalId", new { PeriodicalId = id });
        }

        public IEnumerable<TagDto> GetTagsByArticle(long id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT t.* FROM Tag t
                                INNER JOIN ArticleTag at ON t.Id = at.TagId
                                WHERE at.ArticleId = @ArticleId ", new { ArticleId = id });
        }

        public IEnumerable<TagDto> GetTagsByIssue(int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT t.* FROM Tag t
                                INNER JOIN IssueTag it ON t.Id = it.TagId
                                WHERE it.IssueId = @IssueId ", new { IssueId = id });
        }

        public void AddBooksToTag(IEnumerable<BookDto> books, TagDto tag)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var book in books)
            {
                connection.Execute("INSERT INTO BookTag (BookId, TagId) VALUES(@BookId, @TagId)",
                    new { BookId = book.Id, TagId = tag.Id });
            }
        }

        public void AddBookToTags(int bookId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO BookTag (BookId, TagId) VALUES(@BookId, @TagId)",
                    new { BookId = bookId, TagId = category.Id });
            }
        }

        public void AddPeriodicalToTag(IEnumerable<PeriodicalDto> periodicals, TagDto tag)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var periodical in periodicals)
            {
                connection.Execute("INSERT INTO PeriodicalTag (PeriodicalId, TagId) VALUES(@PeriodicalId, @TagId)",
                    new { PeriodicalId = periodical.Id, TagId = tag.Id });
            }
        }

        public void AddPeriodicalToTags(int periodicalId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO PeriodicalTag (PeriodicalId, TagId) VALUES(@PeriodicalId, @TagId)",
                    new { PeriodicalId = periodicalId, TagId = category.Id });
            }
        }

        public void AddIssueToTags(long issueId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO IssueTag (IssueId, TagId) VALUES(@IssueId, @TagId)",
                    new { IssueId = issueId, TagId = category.Id });
            }
        }

        public void AddArticleToTags(long articleId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO ArticleTag (ArticleId, TagId) VALUES(@ArticleId, @TagId)",
                    new { ArticleId = articleId, TagId = category.Id });
            }
        }
    }
    public class SqlServerTagTestRepository : ITagTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerTagTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        public void AddTag(TagDto tag)
        {
            using var connection = _connectionProvider.GetConnection();
            var id = connection.ExecuteScalar<int>("INSERT INTO Tag (Name, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @LibraryId)", tag);
            tag.Id = id;
        }

        public void AddTags(IEnumerable<TagDto> tags)
        {
            foreach (var category in tags)
            {
                AddTag(category);
            }
        }

        public void DeleteTags(IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            var sql = "DELETE FROM Tag WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = tags.Select(a => a.Id) });
        }

        public TagDto GetTagById(int libraryId, int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QuerySingleOrDefault<TagDto>("SELECT * FROM Tag WHERE Id = @Id AND LibraryId = @LibraryId",
                new { Id = id, LibraryId = libraryId });
        }

        public bool DoesTagExists(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TagDto> GetTags(params int[] ids)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>("SELECT * FROM Tag WHERE Id IN @TagIds",
                new { TagIds = ids });
        }
        
        public IEnumerable<TagDto> GetTagsByBook(int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT c.* FROM Tag c
                                INNER JOIN BookTag bc ON c.Id = bc.TagId
                                WHERE bc.BookId = @BookId ", new { BookId = id });
        }

        public IEnumerable<TagDto> GetTagsByPeriodical(int id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT c.* From Tag c
                                INNER JOIN PeriodicalTag pc ON c.Id = pc.TagId
                                WHERE pc.PeriodicalId = @PeriodicalId", new { PeriodicalId = id });
        }

        public IEnumerable<TagDto> GetTagsByArticle(long id)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<TagDto>(@"SELECT c.* FROM Tag c
                                INNER JOIN ArticleTag bc ON c.Id = bc.TagId
                                WHERE bc.ArticleId = @ArticleId ", new { ArticleId = id });
        }

        public void AddBooksToTag(IEnumerable<BookDto> books, TagDto tag)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var book in books)
            {
                connection.Execute("INSERT INTO BookTag (BookId, TagId) VALUES(@BookId, @TagId)",
                    new { BookId = book.Id, TagId = tag.Id });
            }
        }

        public void AddBookToTags(int bookId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO BookTag (BookId, TagId) VALUES(@BookId, @TagId)",
                    new { BookId = bookId, TagId = category.Id });
            }
        }

        public void AddPeriodicalToTag(IEnumerable<PeriodicalDto> periodicals, TagDto tag)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var periodical in periodicals)
            {
                connection.Execute("INSERT INTO PeriodicalTag (PeriodicalId, TagId) VALUES(@PeriodicalId, @TagId)",
                    new { PeriodicalId = periodical.Id, TagId = tag.Id });
            }
        }

        public void AddPeriodicalToTags(int periodicalId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO PeriodicalTag (PeriodicalId, TagId) VALUES(@PeriodicalId, @TagId)",
                    new { PeriodicalId = periodicalId, TagId = category.Id });
            }
        }

        public void AddIssueToTags(long issueId, IEnumerable<TagDto> tags)
        {
            throw new NotImplementedException();
        }

        public void AddArticleToTags(long articleId, IEnumerable<TagDto> tags)
        {
            using var connection = _connectionProvider.GetConnection();
            foreach (var category in tags)
            {
                connection.Execute("INSERT INTO ArticleTag (ArticleId, TagId) VALUES(@ArticleId, @TagId)",
                    new { ArticleId = articleId, TagId = category.Id });
            }
        }

        public IEnumerable<TagDto> GetTagsByIssue(int viewId)
        {
            throw new NotImplementedException();
        }
    }

    public static class TagDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddTag(this IDbConnection connection, TagDto category)
        {
            var id = _dbType == DatabaseTypes.SqlServer
                ? connection.ExecuteScalar<int>("INSERT INTO Tag (Name, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @LibraryId)", category)
                : connection.ExecuteScalar<int>("INSERT INTO Tag (`Name`, LibraryId) VALUES (@Name, @LibraryId); SELECT LAST_INSERT_ID();", category);
            category.Id = id;
        }

        public static void AddTags(this IDbConnection connection, IEnumerable<TagDto> categories)
        {
            foreach (var category in categories)
            {
                AddTag(connection, category);
            }
        }

        public static void DeleteCategries(this IDbConnection connection, IEnumerable<TagDto> categories)
        {
            var sql = "DELETE FROM Tag WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = categories.Select(a => a.Id) });
        }

        public static TagDto GetTagById(this IDbConnection connection, int libraryId, int id)
        {
            return connection.QuerySingleOrDefault<TagDto>("SELECT * FROM Tag WHERE Id = @Id AND LibraryId = @LibraryId",
                new { Id = id, LibraryId = libraryId });
        }

        public static bool DoesTagExists(this IDbConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<TagDto> GetTagsByBook(this IDbConnection connection, int id)
        {
            return connection.Query<TagDto>(@"SELECT c.* FROM Tag c
                                INNER JOIN BookTag bc ON c.Id = bc.TagId
                                WHERE bc.BookId = @BookId ", new { BookId = id });
        }

        public static IEnumerable<TagDto> GetTagsByPeriodical(this IDbConnection connection, int id)
        {
            return connection.Query<TagDto>(@"SELECT c.* From Tag c
                                INNER JOIN PeriodicalTag pc ON c.Id = pc.TagId
                                WHERE pc.PeriodicalId = @PeriodicalId", new { PeriodicalId = id });
        }

        public static IEnumerable<TagDto> GetTagsByArticle(this IDbConnection connection, long id)
        {
            return connection.Query<TagDto>(@"SELECT c.* FROM Tag c
                                INNER JOIN ArticleTag bc ON c.Id = bc.TagId
                                WHERE bc.ArticleId = @ArticleId ", new { ArticleId = id });
        }

        public static void AddBooksToTag(this IDbConnection connection, IEnumerable<BookDto> books, TagDto category)
        {
            foreach (var book in books)
            {
                connection.Execute("INSERT INTO BookTag (BookId, TagId) VALUES(@BookId, @TagId)",
                    new { BookId = book.Id, TagId = category.Id });
            }
        }

        public static void AddBookToTags(this IDbConnection connection, int bookId, IEnumerable<TagDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("INSERT INTO BookTag (BookId, TagId) VALUES(@BookId, @TagId)",
                    new { BookId = bookId, TagId = category.Id });
            }
        }

        public static void AddPeriodicalToTag(this IDbConnection connection, IEnumerable<PeriodicalDto> periodicals, TagDto category)
        {
            foreach (var periodical in periodicals)
            {
                connection.Execute("INSERT INTO PeriodicalTag (PeriodicalId, TagId) VALUES(@PeriodicalId, @TagId)",
                    new { PeriodicalId = periodical.Id, TagId = category.Id });
            }
        }

        public static void AddPeriodicalToTags(this IDbConnection connection, int periodicalId, IEnumerable<TagDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("INSERT INTO PeriodicalTag (PeriodicalId, TagId) VALUES(@PeriodicalId, @TagId)",
                    new { PeriodicalId = periodicalId, TagId = category.Id });
            }
        }

        public static void AddArticleToTags(this IDbConnection connection, long articleId, IEnumerable<TagDto> categories)
        {
            foreach (var category in categories)
            {
                connection.Execute("INSERT INTO ArticleTag (ArticleId, TagId) VALUES(@ArticleId, @TagId)",
                    new { ArticleId = articleId, TagId = category.Id });
            }
        }
    }
}
