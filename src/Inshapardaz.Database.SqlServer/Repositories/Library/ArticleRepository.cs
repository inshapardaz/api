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
                var sql = "Insert Into Article (Title, IssueId, AuthorId, SequenceNumber, SeriesName, SeriesIndex) Output Inserted.Id Values (@Title, @IssueId, @AuthorId, @SequenceNumber, @SeriesName, @SeriesIndex)";
                var command = new CommandDefinition(sql, new
                {
                    Title = article.Title,
                    IssueId = article.IssueId,
                    AuthorId = article.AuthorId,
                    SequenceNumber = article.SequenceNumber,
                    SeriesName = article.SeriesName,
                    SeriesIndex = article.SeriesIndex
                }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetArticleById(libraryId, peridicalId, issueId, id, cancellationToken);
        }

        public async Task UpdateArticle(int libraryId, int periodicalId, int issueId, ArticleModel article, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update A Set A.Title = @Title, A.IssueId = @IssueId, A.AuthorId = @AuthorId, A.SequenceNumber = @SequenceNumber
                            A.SeriesName = @SeriesName, A.SeriesIndex = @SeriesIndex
                            From Chapter A
                            Inner Join Issue i On i.Id = A.IssueId
                            Where A.Id = @ArticleId AND A.IssueId= @OldIssueId And i.PeriodicalId = @PeriodicalId";
                var args = new
                {
                    LibraryId = libraryId,
                    OldIssueId = issueId,
                    ArticleId = article.Id,
                    PeriodicalId = periodicalId,
                    Title = article.Title,
                    IssueId = article.IssueId,
                    AuthorId = article.AuthorId,
                    SequenceNumber = article.SequenceNumber,
                    SeriesName = article.SeriesName,
                    SeriesIndex = article.SeriesIndex
                };
                var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteArticle(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete a From Article a
                            Inner Join Issue i On i.Id = a.IssueId
                            Where c.Id = @ArticleId AND c.IssueId = @IssueId And b.PeriodicalId = @PeriodicalId";
                var command = new CommandDefinition(sql, new
                {
                    PeriodicalId = periodicalId,
                    IssueId = issueId,
                    ArticleId = articleId
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<ArticleModel> GetArticleById(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ArticleModel article = null;
                var sql = @"Select a.*, ac.*, f.*
                            From Article a
                            Inner Join Issue i On i.Id = a.IssueId
                            Left Outer Join ArticleContent ac On a.Id = ac.ArticleId
                            Left Outer Join [File] f On f.Id = cc.FileId
                            Where c.Id = @ArticleId AND i.Id = @IssueId";
                var command = new CommandDefinition(sql, new { IssueId = issueId, ArticleId = articleId }, cancellationToken: cancellationToken);
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

        public async Task<ArticleContentModel> GetArticleContentById(int libraryId, int periodicalId, int issueId, int articleId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                ArticleModel article = null;
                var sql = @"Select a.*, ac.*, f.*
                            From Article a
                            Inner Join Issue i On i.Id = a.IssueId
                            Left Outer Join ArticleContent ac On a.Id = ac.ArticleId
                            Left Outer Join [File] f On f.Id = cc.FileId
                            Where c.Id = @ArticleId AND i.Id = @IssueId AMD a.MimeType = @MimeType AND a.Language = @Language";
                var command = new CommandDefinition(sql, new { IssueId = issueId, ArticleId = articleId, MimeType = mimeType, Language = language }, cancellationToken: cancellationToken);
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

        public async Task<IEnumerable<ArticleModel>> GetArticlesByIssue(int libraryId, int periodicalId, int issueId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var articles = new Dictionary<int, ArticleModel>();

                var sql = @"Select a.*, ac.*, f.*  From Article a
                            Inner Join Issue i On i.Id = b.IssueId
                            Left Outer Join ArticleContent ac On a.Id = ac.ArticleId
                            Left Outer Join [File] f On f.Id = cc.FileId
                            Where a.PeriodicalId = @PeriodicalId AND a.IssueId = @IssueId";
                var command = new CommandDefinition(sql, new { PeriodicalId = periodicalId, IssueId = issueId }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, FileModel, ArticleModel>(command, (a, ac, f) =>
                {
                    if (!articles.TryGetValue(a.Id, out ArticleModel article))
                    {
                        articles.Add(a.Id, article = a);
                    }

                    article = articles[a.Id];
                    if (ac != null)
                    {
                        ac.IssueId = issueId;
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

                return articles.Values;
            }
        }

        public Task<ArticleContentModel> GetArticleContent(int libraryId, int periodicalId, int issueId, int articleId, string language, string mimeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetArticleContentUrl(int libraryId, int periodicalId, int issueId, int articleId, string language, string mimeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleContentModel> AddArticleContent(int libraryId, ArticleContentModel issueContent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteArticleContentById(int libraryId, int periodicalId, int issueId, int articleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
