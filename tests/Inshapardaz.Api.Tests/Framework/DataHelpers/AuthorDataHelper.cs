using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IAuthorTestRepository
    {
        void AddAuthor(AuthorDto author);
        void AddAuthors(IEnumerable<AuthorDto> authors);
        void DeleteAuthor(int authorId);
        void DeleteAuthors(IEnumerable<AuthorDto> authors);
        AuthorDto GetAuthorById(int id);
        string GetAuthorImageUrl(int id);
        FileDto GetAuthorImage(int id);
        IEnumerable<AuthorDto> GetAuthorsByBook(int bookId);
        IEnumerable<AuthorDto> GetAuthorsByArticle(long articleId);
        IEnumerable<AuthorDto> GetAuthorsByIssueArticle(long issueArticleId);
    }
    public class MySqlAuthorTestRepository : IAuthorTestRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public MySqlAuthorTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddAuthor(AuthorDto author)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Author (`Name`, `ImageId`, LibraryId) VALUES (@Name, @ImageId, @LibraryId); SELECT LAST_INSERT_ID();", author); ;
                author.Id = id;
            }
        }

        public void AddAuthors(IEnumerable<AuthorDto> authors)
        {
            foreach (var author in authors)
            {
                AddAuthor(author);
            }
        }

        public void DeleteAuthor(int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Author WHERE Id = @Id";
                connection.Execute(sql, new { Id = authorId });
            }
        }

        public void DeleteAuthors(IEnumerable<AuthorDto> authors)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Author WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = authors.Select(a => a.Id) });
            }
        }

        public AuthorDto GetAuthorById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<AuthorDto>("SELECT * FROM Author WHERE Id = @Id", new { Id = id });
            }
        }

        public string GetAuthorImageUrl(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath from `File` f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
            }
        }

        public FileDto GetAuthorImage(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* from `File` f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
            }
        }

        public IEnumerable<AuthorDto> GetAuthorsByBook(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var query = @"SELECT * From Author
                    INNER JOIN BookAuthor ba ON Id = ba.AuthorId
                    WHERE ba.BookId = @Id";
                return connection.Query<AuthorDto>(query, new { Id = bookId });
            }
        }

        public IEnumerable<AuthorDto> GetAuthorsByArticle(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var query = @"SELECT a.* From ArticleAuthor
                    INNER JOIN Author a ON a.Id = ArticleAuthor.AuthorId
                    WHERE ArticleAuthor.ArticleId = @Id";
                return connection.Query<AuthorDto>(query, new { Id = articleId });
            }
        }

        public IEnumerable<AuthorDto> GetAuthorsByIssueArticle(long issueArticleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var query = @"SELECT a.* From IssueArticleAuthor
                    INNER JOIN Author a ON a.Id = IssueArticleAuthor.AuthorId
                    WHERE IssueArticleAuthor.IssueArticleId = @Id";
                return connection.Query<AuthorDto>(query, new { Id = issueArticleId });
            }
        }
    }

    public class SqlServerAuthorTestRepository : IAuthorTestRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public SqlServerAuthorTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddAuthor(AuthorDto author)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("Insert Into Author (Name, ImageId, LibraryId) OUTPUT Inserted.Id VALUES (@Name, @ImageId, @LibraryId)", author);
                author.Id = id;
            }
        }

        public void AddAuthors(IEnumerable<AuthorDto> authors)
        {
            foreach (var author in authors)
            {
                AddAuthor(author);
            }
        }

        public void DeleteAuthor(int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Author WHERE Id = @Id";
                connection.Execute(sql, new { Id = authorId });
            }
        }

        public void DeleteAuthors(IEnumerable<AuthorDto> authors)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Author WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = authors.Select(a => a.Id) });
            }
        }

        public AuthorDto GetAuthorById(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<AuthorDto>("SELECT * FROM Author WHERE Id = @Id", new { Id = id });
            }
        }

        public string GetAuthorImageUrl(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.FilePath from [File] f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    Where a.Id = @Id";
                return connection.QuerySingleOrDefault<string>(sql, new { Id = id });
            }
        }

        public FileDto GetAuthorImage(int id)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT f.* from [File] f
                    INNER JOIN Author a ON f.Id = a.ImageId
                    WHERE a.Id = @Id";
                return connection.QuerySingleOrDefault<FileDto>(sql, new { Id = id });
            }
        }

        public IEnumerable<AuthorDto> GetAuthorsByBook(int bookId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var query = @"SELECT * From Author
                    INNER JOIN BookAuthor ba ON Id = ba.AuthorId
                    WHERE ba.BookId = @Id";
                return connection.Query<AuthorDto>(query, new { Id = bookId });
            }
        }

        public IEnumerable<AuthorDto> GetAuthorsByArticle(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var query = @"SELECT a.* From ArticleAuthor
                    INNER JOIN Author a ON a.Id = ArticleAuthor.AuthorId
                    WHERE ArticleAuthor.ArticleId = @Id";
                return connection.Query<AuthorDto>(query, new { Id = articleId });
            }
        }
        
        public IEnumerable<AuthorDto> GetAuthorsByIssueArticle(long issueArticleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var query = @"SELECT a.* From IssueArticleAuthor
                    INNER JOIN Author a ON a.Id = IssueArticleAuthor.AuthorId
                    WHERE IssueArticleAuthor.IssueArticleId = @Id";
                return connection.Query<AuthorDto>(query, new { Id = issueArticleId });
            }
        }
    }

}
