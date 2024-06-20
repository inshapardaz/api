using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IIssueArticleTestRepository
    {
        void AddIssueArticle(IssueArticleDto issue);

        void AddIssueArticles(IEnumerable<IssueArticleDto> issues);

        IssueArticleDto GetIssueArticleById(int articleId);

        IssueArticleDto GetIssueArticleById(long articleId);

        IssueArticleDto GetIssueArticlesByIssue(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber);

        void DeleteIssueArticles(IEnumerable<IssueArticleDto> articles);

        IEnumerable<IssueArticleDto> GetIssueArticlesByIssue(int issueId);

        void AddIssueArticleAuthor(int issueId, int authorId);

        IssueArticleContentDto GetIssueArticleContent(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language);

        IEnumerable<IssueArticleContentDto> GetIssueArticleContents(long articleId);

        IEnumerable<IssueArticleContentDto> GetContentByIssueArticle(int articleId);

        IEnumerable<IssueArticleContentDto> GetIssueContentByArticle(long articleId);

        void AddIssueArticleContents(IEnumerable<IssueArticleContentDto> contents);

        void AddIssueArticleContents(IssueArticleContentDto content);
    }

    public class MySqlIssueArticleTestRepository : IIssueArticleTestRepository
    {
        private IProvideConnection _connectionProvider;

        public MySqlIssueArticleTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddIssueArticle(IssueArticleDto issue)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssueArticle (Title, IssueId, SequenceNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SeriesName, SeriesIndex)
                    VALUES (@Title, @IssueId, @SequenceNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SeriesName, @SeriesIndex);
                    SELECT LAST_INSERT_ID();";
                var id = connection.ExecuteScalar<int>(sql, issue);
                issue.Id = id;
            }
        }

        public void AddIssueArticles(IEnumerable<IssueArticleDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssueArticle(issue);
            }
        }

        public IssueArticleDto GetIssueArticleById(int articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM IssueArticle WHERE Id = @Id";
                return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new { Id = articleId });
            }
        }

        public IssueArticleDto GetIssueArticleById(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM IssueArticle WHERE Id = @Id";
                return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new { Id = articleId });
            }
        }

        public IssueArticleDto GetIssueArticlesByIssue(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.* FROM IssueArticle a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber";
                return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                });
            }
        }

        public void DeleteIssueArticles(IEnumerable<IssueArticleDto> articles)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM IssueArticle WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
            }
        }

        public IEnumerable<IssueArticleDto> GetIssueArticlesByIssue(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.* FROM IssueArticle a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.Id = @IssueId";
                return connection.Query<IssueArticleDto>(sql, new { IssueId = issueId });
            }
        }

        public void AddIssueArticleAuthor(int issueId, int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO IssueArticleAuthor VALUES (@IssueId, @AuthorId)";
                connection.Execute(sql, new { IssueId = issueId, AuthorId = authorId });
            }
        }

        public IssueArticleContentDto GetIssueArticleContent(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<IssueArticleContentDto>(@"SELECT ac.*
                    FROM IssueArticle a
                        INNER JOIN Issue i ON i.Id = a.IssueId
                        INNER JOIN Periodical p ON p.Id = i.Periodicalid
                    LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
                    WHERE i.PeriodicalId = @PeriodicalId
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND a.SequenceNumber = @SequenceNumber
                        AND ac.Language = @Language",
                new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language
                });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetIssueArticleContents(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>(@"SELECT *
                    FROM IssueArticleContent
                    WHERE ArticleId = @Id",
                new
                {
                    Id = articleId
                });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetContentByIssueArticle(int articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>("SELECT * FROM IssueArticleContent WHERE ArticleId = @Id", new { Id = articleId });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetIssueContentByArticle(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>("SELECT * FROM IssueArticleContent WHERE ArticleId = @Id", new { Id = articleId });
            }
        }

        public void AddIssueArticleContents(IEnumerable<IssueArticleContentDto> contents)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO IssueArticleContent (ArticleId, Language, FileId) VALUES (@ArticleId, @Language, @FileId)";
                connection.Execute(sql, contents);
            }
        }

        public void AddIssueArticleContents(IssueArticleContentDto content)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO ArticleContent (ArticleId, Language, FileId) VALUES (@ArticleId, @Language, @FileId)";
                connection.Execute(sql, content);
            }
        }

    }

    public class SqlServerIssueArticleTestRepository : IIssueArticleTestRepository
    {
        private IProvideConnection _connectionProvider;

        public SqlServerIssueArticleTestRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void AddIssueArticle(IssueArticleDto issue)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssueArticle (Title, IssueId, SequenceNumber, Status, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp, SeriesName, SeriesIndex)
                    OUTPUT INSERTED.ID
                    VALUES (@Title, @IssueId, @SequenceNumber, @Status, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp, @SeriesName, @SeriesIndex)";
                var id = connection.ExecuteScalar<int>(sql, issue);
                issue.Id = id;
            }
        }

        public void AddIssueArticles(IEnumerable<IssueArticleDto> issues)
        {
            foreach (var issue in issues)
            {
                AddIssueArticle(issue);
            }
        }

        public IssueArticleDto GetIssueArticleById(int articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM IssueArticle WHERE Id = @Id";
                return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new { Id = articleId });
            }
        }

        public IssueArticleDto GetIssueArticleById(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM IssueArticle WHERE Id = @Id";
                return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new { Id = articleId });
            }
        }

        public IssueArticleDto GetIssueArticlesByIssue(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.* FROM IssueArticle a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber";
                return connection.QuerySingleOrDefault<IssueArticleDto>(sql, new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                });
            }
        }

        public void DeleteIssueArticles(IEnumerable<IssueArticleDto> articles)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM IssueArticle WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = articles.Select(f => f.Id) });
            }
        }

        public IEnumerable<IssueArticleDto> GetIssueArticlesByIssue(int issueId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT a.* FROM IssueArticle a
                        INNER JOIN Issue i on i.Id = a.IssueId
                        WHERE i.Id = @IssueId";
                return connection.Query<IssueArticleDto>(sql, new { IssueId = issueId });
            }
        }

        public void AddIssueArticleAuthor(int issueId, int authorId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO IssueArticleAuthor VALUES (@IssueId, @AuthorId)";
                connection.Execute(sql, new { IssueId = issueId, AuthorId = authorId });
            }
        }

        public IssueArticleContentDto GetIssueArticleContent(int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<IssueArticleContentDto>(@"SELECT ac.*
                    FROM IssueArticle a
                        INNER JOIN Issue i ON i.Id = a.IssueId
                        INNER JOIN Periodical p ON p.Id = i.Periodicalid
                    LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
                    WHERE i.PeriodicalId = @PeriodicalId
                        AND i.VolumeNumber = @VolumeNumber
                        AND i.IssueNumber = @IssueNumber
                        AND a.SequenceNumber = @SequenceNumber
                        AND ac.Language = @Language",
                new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language
                });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetIssueArticleContents(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>(@"SELECT *
                    FROM IssueArticleContent
                    WHERE ArticleId = @Id",
                new
                {
                    Id = articleId
                });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetContentByIssueArticle(int articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>("SELECT * FROM IssueArticleContent WHERE ArticleId = @Id", new { Id = articleId });
            }
        }

        public IEnumerable<IssueArticleContentDto> GetIssueContentByArticle(long articleId)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                return connection.Query<IssueArticleContentDto>("SELECT * FROM IssueArticleContent WHERE ArticleId = @Id", new { Id = articleId });
            }
        }

        public void AddIssueArticleContents(IEnumerable<IssueArticleContentDto> contents)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO IssueArticleContent (ArticleId, Language, FileId) VALUES (@ArticleId, @Language, @FileId)";
                connection.Execute(sql, contents);
            }
        }

        public void AddIssueArticleContents(IssueArticleContentDto content)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "INSERT INTO ArticleContent (ArticleId, Language, FileId) VALUES (@ArticleId, @Language, @FileId)";
                connection.Execute(sql, content);
            }
        }

    }
}
