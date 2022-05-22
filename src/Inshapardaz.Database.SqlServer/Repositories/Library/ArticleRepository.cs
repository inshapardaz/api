using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public ArticleRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<ArticleModel> AddArticle(int libraryId, int peridicalId, int issueId, ArticleModel article, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO Article (Title, IssueId, AuthorId, SequenceNumber, SeriesName, SeriesIndex) 
                            OUTPUT Inserted.Id VALUES (@Title, @IssueId, @AuthorId, @SequenceNumber, @SeriesName, @SeriesIndex)";
                var command = new CommandDefinition(sql, new
                {
                    Title = article.Title,
                    IssueId = issueId,
                    AuthorId = article.AuthorId,
                    SequenceNumber = article.SequenceNumber,
                    SeriesName = article.SeriesName,
                    SeriesIndex = article.SeriesIndex
                }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetArticleById(libraryId, peridicalId, issueId, id, cancellationToken);
        }

        public async Task UpdateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, ArticleModel article, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE a SET a.Title = @Title, a.AuthorId = @AuthorId, a.SequenceNumber = @SequenceNumber
                            a.SeriesName = @SeriesName, a.SeriesIndex = @SeriesIndex
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            WHERE a.Id = @ArticleId 
                            AND i.VolumeNumber = @VolumeNumber 
                            AND i.IssueNumber = @IssueNumber 
                            AND i.PeriodicalId = @PeriodicalId";
                var args = new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    ArticleId = article.Id,
                    Title = article.Title,
                    AuthorId = article.AuthorId,
                    SequenceNumber = article.SequenceNumber,
                    SeriesName = article.SeriesName,
                    SeriesIndex = article.SeriesIndex
                };
                var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"DELETE a 
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            WHERE a.Id = @ArticleId 
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber";
                var command = new CommandDefinition(sql, new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    ArticleId = articleId
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<ArticleModel> GetArticleById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ArticleModel article = null;
                var sql = @"SELECT a.*, ac.*, f.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            LEFT OUTER JOIN [File] f ON f.Id = cc.FileId
                            WHERE a.Id = @ArticleId
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    ArticleId = articleId
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, FileModel, ArticleModel>(command, (a, ac, f) =>
                {
                    if (article == null)
                    {
                        article = a;
                    }

                    if (ac != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content == null)
                        {
                            article.Contents.Add(ac);
                        }
                    }

                    if (f != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content != null)
                        {
                            content.MimeType = f.MimeType;
                            content.ContentUrl = f.FilePath;
                        }
                    }

                    return article;
                });

                return article;
            }
        }


        public async Task<ArticleContentModel> GetArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ArticleModel article = null;
                var sql = @"SELECT a.*, ac.*, f.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            LEFT OUTER JOIN [File] f ON f.Id = cc.FileId
                            WHERE a.Id = @ArticleId
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.MimeType = @MimeType 
                            AND a.Language = @Language";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    ArticleId = articleId,
                    MimeType = mimeType, 
                    Language = language }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, FileModel, ArticleModel>(command, (a, ac, f) =>
                {
                    if (article == null)
                    {
                        article = a;
                    }

                    if (ac != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content == null)
                        {
                            article.Contents.Add(ac);
                        }
                    }

                    if (f != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content != null)
                        {
                            content.MimeType = f.MimeType;
                            content.ContentUrl = f.FilePath;
                        }
                    }

                    return article;
                });

                return null;
            }
        }

        public async Task<IEnumerable<ArticleModel>> GetArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var articles = new Dictionary<int, ArticleModel>();

                var sql = @"SELECT a.*, at.* 
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleText at ON a.Id = at.ArticleId
                            WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, ArticleModel>(command, (a, at) =>
                {
                    if (!articles.TryGetValue(a.Id, out ArticleModel article))
                    {
                        articles.Add(a.Id, article = a);
                    }

                    article = articles[a.Id];
                    if (at != null)
                    {
                        at.IssueId = a.Id;
                        var content = article.Contents.SingleOrDefault(x => x.Id == at.Id);
                        if (content == null)
                        {
                            article.Contents.Add(at);
                        }
                    }

                    return article;
                });

                return articles.Values;
            }
        }

        public Task<ArticleContentModel> GetArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetArticleContentUrl(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleContentModel> AddArticleContent(int libraryId, ArticleContentModel issueContent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<ArticleModel> GetArticleById(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ArticleModel article = null;
                var sql = @"SELECT a.*, ac.*, f.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            LEFT OUTER JOIN [File] f ON f.Id = cc.FileId
                            WHERE a.Id = @ArticleId
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.Id = @IssueId";
                var command = new CommandDefinition(sql, new
                {
                    PeriodicalId = periodicalId,
                    IssueId = issueId,
                    ArticleId = articleId
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, FileModel, ArticleModel>(command, (a, ac, f) =>
                {
                    if (article == null)
                    {
                        article = a;
                    }

                    if (ac != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content == null)
                        {
                            article.Contents.Add(ac);
                        }
                    }

                    if (f != null)
                    {
                        var content = article.Contents.SingleOrDefault(x => x.Id == ac.Id);
                        if (content != null)
                        {
                            content.MimeType = f.MimeType;
                            content.ContentUrl = f.FilePath;
                        }
                    }

                    return article;
                });

                return article;
            }
        }
    }
}
