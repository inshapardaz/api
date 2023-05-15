using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async Task<ArticleModel> AddArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, ArticleModel article, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO Article (Title, IssueId, Status, SequenceNumber, SeriesName, SeriesIndex, WriterAccountId, WriterAssignTimestamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                            OUTPUT Inserted.Id VALUES (@Title, (SELECT Id FROM Issue WHERE VolumeNumber = @VolumeNumber AND IssueNumber = @IssueNumber), @Status, @SequenceNumber, @SeriesName, @SeriesIndex, @WriterAccountId, @WriteAssignTimestamp, @ReviewerAccountId, @ReviewerAssignTimeStamp)";
                var command = new CommandDefinition(sql, new
                {
                    Title = article.Title,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = article.SequenceNumber,
                    Status = article.Status,
                    SeriesName = article.SeriesName,
                    SeriesIndex = article.SeriesIndex,
                    WriterAccountId = article.WriterAccountId,
                    WriteAssignTimestamp = article.WriterAssignTimeStamp,
                    ReviewerAccountId = article.ReviewerAccountId,
                    ReviewerAssignTimeStamp = article.ReviewerAssignTimeStamp
                }, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);

                var sqlAuthor = @"Insert Into ArticleAuthor (ArticleId, AuthorId) Values (@ArticleId, @AuthorId);";

                if (article.Authors != null && article.Authors.Any())
                {
                    var bookAuthors = article.Authors.Select(a => new { ArticleId = article.Id, AuthorId = a.Id });
                    var commandCategory = new CommandDefinition(sqlAuthor, bookAuthors, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }
            }

            await ReorderArticles(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
            return await GetArticleById(id, cancellationToken);
        }

        public async Task UpdateArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, ArticleModel article, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE a SET a.Title = @Title, 
                            a.SequenceNumber = @SequenceNumber,
                            a.SeriesName = @SeriesName, 
                            a.SeriesIndex = @SeriesIndex,
                            a.WriterAccountId = @WriterAccountId, 
                            a.WriterAssignTimestamp = @WriteAssignTimestamp, 
                            a.ReviewerAccountId = @ReviewerAccountId, 
                            a.ReviewerAssignTimeStamp = @ReviewerAssignTimeStamp
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId 
                            AND p.Id = @PeriodicalId 
                            AND i.VolumeNumber = @VolumeNumber 
                            AND i.IssueNumber = @IssueNumber";
                var args = new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    ArticleId = article.Id,
                    Title = article.Title,
                    SequenceNumber = article.SequenceNumber,
                    SeriesName = article.SeriesName,
                    SeriesIndex = article.SeriesIndex,
                    WriterAccountId = article.WriterAccountId,
                    WriteAssignTimestamp = article.WriterAssignTimeStamp,
                    ReviewerAccountId = article.ReviewerAccountId,
                    ReviewerAssignTimeStamp = article.ReviewerAssignTimeStamp
                };
                var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                await ReorderArticles(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
            }
        }

        public async Task DeleteArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
        {
            await DeleteArticleContent(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"DELETE a 
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);

                await ReorderArticles(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
            }
        }

        public async Task<ArticleModel> GetArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                ArticleModel article = null;
                var sql = @"SELECT a.*, ac.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, ArticleModel>(command, (a, ac) =>
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

                    return article;
                });

                return article;
            }
        }

        public async Task<ArticleContentModel> GetArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                ArticleContentModel articleContent = null;
                var sql = @"SELECT a.*, ac.*, i.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber
                            AND ac.Language = @Language";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, IssueModel, ArticleContentModel>(command, (a, ac, i) =>
                {
                    if (articleContent == null)
                    {
                        articleContent = ac;
                        articleContent.PeriodicalId = i.PeriodicalId;
                        articleContent.VolumeNumber = i.VolumeNumber;
                        articleContent.IssueNumber = i.IssueNumber;
                        articleContent.SequenceNumber = a.SequenceNumber;
                    }

                    return articleContent;
                });

                return articleContent;
            }
        }

        public async Task<IEnumerable<ArticleModel>> GetArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var articles = new Dictionary<int, ArticleModel>();

                var sql = @"SELECT a.*, at.*, i.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent at ON a.Id = at.ArticleId
                            WHERE i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            ORDER BY a.SequenceNumber";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, IssueModel, ArticleModel>(command, (a, at, i) =>
                {
                    if (!articles.TryGetValue(a.Id, out ArticleModel article))
                    {
                        articles.Add(a.Id, article = a);
                    }

                    article = articles[a.Id];
                    if (at != null)
                    {
                        at.VolumeNumber = i.VolumeNumber;
                        at.IssueNumber = i.IssueNumber;

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

        public async Task<ArticleContentModel> GetArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT ac.*, a.sequenceNumber, p.Id AS PeriodicalId 
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            INNER JOIN Periodical p ON p.Id = i.Periodicalid
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE p.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND a.sequenceNumber = @SequenceNumber 
                                AND ac.Language = @Language";
                var command = new CommandDefinition(sql, new { 
                    LibraryId = libraryId, 
                    PeriodicalId = periodicalId, 
                    VolumeNumber = volumeNumber, 
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language 
                }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ArticleContentModel>(command);
            }
        }

        public async Task<IEnumerable<ArticleContentModel>> GetArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT ac.*, a.SequenceNumber AS SequenceNumber, p.Id AS PeriodicalId 
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            INNER JOIN Periodical p ON p.Id = i.Periodicalid
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE a.sequenceNumber = @SequenceNumber 
                                AND p.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND a.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<ArticleContentModel>(command);
            }
        }

        public async Task<ArticleContentModel> AddArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, string content, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO  ArticleContent (ArticleId, [Language], Text)
                            (SELECT TOp 1 a.Id, @Language,  @Text FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId =  @LibraryId
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber)"
                ;
                var command = new CommandDefinition(sql, new {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language, 
                    Text = content }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
                return await GetArticleContentById(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, language, cancellationToken);
            }
        }

        public async Task<ArticleContentModel> UpdateArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, string content, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update ac SET Text = @Text
                            FROM ArticleContent ac
                            INNER JOIN Article a ON a.Id = ac.Articleid
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId =  @LibraryId
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND a.SequenceNumber = @SequenceNumber
                            AND ac.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Language = language,
                    Text = content
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }

            return await GetArticleContent(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, language, cancellationToken);
        }

        public async Task DeleteArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Delete ac
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            INNER JOIN Periodical p ON p.Id = i.Periodicalid
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE a.sequenceNumber = @SequenceNumber 
                                AND p.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND a.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public Task<ArticleModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ArticleModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<ArticleModel> GetArticleById(int articleId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                ArticleModel article = null;
                var sql = @"SELECT a.*, ac.*
                            FROM Article a
                            INNER JOIN Issue i ON i.Id = a.IssueId
                            LEFT OUTER JOIN ArticleContent ac ON a.Id = ac.ArticleId
                            WHERE a.Id = @Id";
                var command = new CommandDefinition(sql, new
                {
                    Id = articleId
                }, cancellationToken: cancellationToken);
                await connection.QueryAsync<ArticleModel, ArticleContentModel, ArticleModel>(command, (a, ac) =>
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

                    return article;
                });

                return article;
            }
        }

        public async Task ReorderArticles(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT a.Id, row_number() OVER (ORDER BY a.SequenceNumber) as 'SequenceNumber'
                            From Article a
                            Inner Join Issue i On i.Id = a.IssueId
                            Inner Join Periodical p On p.Id = i.PeriodicalId
                            Where p.LibraryId = @LibraryId 
                            AND p.Id = @PeriodicalId 
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            Order By a.SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber
                }, cancellationToken: cancellationToken);
                var newOrder = await connection.QueryAsync(command);

                var sql2 = @"UPDATE Article
                            SET SequenceNumber = @SequenceNumber
                            Where Id = @Id";
                var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command2);
            }
        }

        public async Task UpdateArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IEnumerable<ArticleModel> articles, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update A Set A.SequenceNumber = @SequenceNumber
                            From Article A
                            Inner Join Issue i On i.Id = A.IssueId
                            Inner Join Periodical p On p.Id = i.PeriodicalId
                            Where p.LibraryId = @LibraryId 
                            AND p.Id = @PeriodicalId 
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND A.Id = @Id";
                var args = articles.Select(a => new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    Id = a.Id,
                    SequenceNumber = a.SequenceNumber
                });
                var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
