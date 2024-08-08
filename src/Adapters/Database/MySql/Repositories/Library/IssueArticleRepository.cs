using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class IssueArticleRepository : IIssueArticleRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public IssueArticleRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IssueArticleModel> AddIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueArticleModel issueArticle, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO IssueArticle (`Title`, IssueId, `Status`, SequenceNumber, SeriesName, SeriesIndex, WriterAccountId, WriterAssignTimeStamp, ReviewerAccountId, ReviewerAssignTimeStamp) 
                            VALUES (@Title, (SELECT Id FROM Issue WHERE VolumeNumber = @VolumeNumber AND IssueNumber = @IssueNumber), @Status, @SequenceNumber, @SeriesName, @SeriesIndex, @WriterAccountId, @WriterAssignTimeStamp, @ReviewerAccountId, @ReviewerAssignTimeStamp);
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, new
            {
                Title = issueArticle.Title,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = int.MaxValue,
                Status = issueArticle.Status,
                SeriesName = issueArticle.SeriesName,
                SeriesIndex = issueArticle.SeriesIndex,
                WriterAccountId = issueArticle.WriterAccountId,
                WriterAssignTimeStamp = issueArticle.WriterAssignTimeStamp,
                ReviewerAccountId = issueArticle.ReviewerAccountId,
                ReviewerAssignTimeStamp = issueArticle.ReviewerAssignTimeStamp
            }, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);

            var sqlAuthor = @"INSERT INTO IssueArticleAuthor (IssueArticleId, AuthorId) VALUES (@ArticleId, @AuthorId);";

            if (issueArticle.Authors != null && issueArticle.Authors.Any())
            {
                var authors = issueArticle.Authors.Select(a => new { ArticleId = id, AuthorId = a.Id });
                var commandCategory = new CommandDefinition(sqlAuthor, authors, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }
        }

        await ReorderArticles(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
        return await GetIssueArticleById(id, cancellationToken);
    }

    public async Task<IssueArticleModel> UpdateIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IssueArticleModel issueArticle, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            SET a.Title = @Title, 
                                a.SequenceNumber = @SequenceNumber,
                                a.SeriesName = @SeriesName, 
                                a.SeriesIndex = @SeriesIndex,
                                a.WriterAccountId = @WriterAccountId, 
                                a.WriterAssignTimeStamp = @WriterAssignTimeStamp, 
                                a.ReviewerAccountId = @ReviewerAccountId, 
                                a.ReviewerAssignTimeStamp = @ReviewerAssignTimeStamp,
                                a.Status = @Status
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
                ArticleId = issueArticle.Id,
                Title = issueArticle.Title,
                SequenceNumber = issueArticle.SequenceNumber,
                SeriesName = issueArticle.SeriesName,
                SeriesIndex = issueArticle.SeriesIndex,
                Status = issueArticle.Status,
                WriterAccountId = issueArticle.WriterAccountId,
                WriterAssignTimeStamp = issueArticle.WriterAssignTimeStamp,
                ReviewerAccountId = issueArticle.ReviewerAccountId,
                ReviewerAssignTimeStamp = issueArticle.ReviewerAssignTimeStamp
            };
            var command = new CommandDefinition(sql, args, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
            
            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM IssueArticleAuthor WHERE IssueArticleId = @IssueArticleId",
                new { IssueArticleId = issueArticle.Id },
                cancellationToken: cancellationToken));

            var sqlAuthor = @"INSERT INTO IssueArticleAuthor (IssueArticleId, AuthorId) 
                                  VALUES (@IssueArticleId, @AuthorId);";

            if (issueArticle.Authors != null && issueArticle.Authors.Any())
            {
                var issueArticleAuthors = issueArticle.Authors.Select(a => new { IssueArticleId = issueArticle.Id, AuthorId = a.Id });
                var commandCategory = new CommandDefinition(sqlAuthor, issueArticleAuthors, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(commandCategory);
            }

            await ReorderArticles(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
            return await GetIssueArticleById(issueArticle.Id, cancellationToken);
        }
    }

    public async Task DeleteIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        await DeleteIssueArticleContent(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE a 
                            FROM IssueArticle a
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

    public async Task<IssueArticleModel> GetIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            IssueArticleModel article = null;
            var sql = @"SELECT a.*, ac.*, au.*
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                LEFT OUTER JOIN IssueArticleAuthor iaa ON iaa.IssueArticleId = a.Id
                                LEFT OUTER JOIN Author au On iaa.AuthorId = au.Id
                                LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
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
            await connection.QueryAsync<IssueArticleModel, IssueArticleContentModel, AuthorModel, IssueArticleModel>(command, (a, ac, iaa) =>
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
                
                if (iaa != null)
                {
                    var author = article.Authors.SingleOrDefault(x => x.Id == iaa.Id);
                    if (author == null)
                    {
                        article.Authors.Add(iaa);
                    }
                }

                return article;
            });

            return article;
        }
    }

    public async Task<IssueArticleContentModel> GetIssueArticleContentById(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string language, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            IssueArticleContentModel articleContent = null;
            var sql = @"SELECT a.*, ac.*, i.*
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
                            WHERE i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND a.SequenceNumber = @SequenceNumber
                                AND ac.Language = @Language";
            var command = new CommandDefinition(sql, new
            {
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber,
                Language = language
            }, cancellationToken: cancellationToken);
            await connection.QueryAsync<IssueArticleModel, IssueArticleContentModel, IssueModel, IssueArticleContentModel>(command, (a, ac, i) =>
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

    public async Task<IEnumerable<IssueArticleModel>> GetIssueArticlesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var articles = new Dictionary<long, IssueArticleModel>();

            var sql = @"SELECT a.*, at.*, i.*, au.*,
                            aw.Name As WriterAccountName, 
                            ar.Name As ReviewerAccountName
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                LEFT OUTER JOIN IssueArticleAuthor iaa ON iaa.IssueArticleId = a.Id
                                LEFT OUTER JOIN Author au On iaa.AuthorId = au.Id
                                LEFT OUTER JOIN IssueArticleContent at ON a.Id = at.ArticleId
                                LEFT OUTER JOIN Accounts aw ON aw.Id = a.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = a.ReviewerAccountId                
                            WHERE i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                            ORDER BY a.SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
            }, cancellationToken: cancellationToken);
            await connection.QueryAsync<IssueArticleModel, IssueArticleContentModel, IssueModel, AuthorModel, string, string, IssueArticleModel>(command, 
                (a, at, i, iaa, WriterAccountName, ReviewerAccountName) =>
            {
                if (!articles.TryGetValue(a.Id, out IssueArticleModel article))
                {
                    a.WriterAccountName = WriterAccountName;
                    a.ReviewerAccountName = ReviewerAccountName;
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
                
                if (iaa != null)
                {
                    var author = article.Authors.SingleOrDefault(x => x.Id == iaa.Id);
                    if (author == null)
                    {
                        article.Authors.Add(iaa);
                    }
                }

                return article;
            }, splitOn: "Id, WriterAccountName,ReviewerAccountName");

            return articles.Values;
        }
    }

    public async Task<IEnumerable<IssueArticleContentModel>> GetIssueArticleContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT ac.*, a.SequenceNumber AS SequenceNumber, p.Id AS PeriodicalId 
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.Periodicalid
                                LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
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
            return await connection.QueryAsync<IssueArticleContentModel>(command);
        }
    }

    public async Task<IssueArticleContentModel> GetIssueArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT ac.*, a.sequenceNumber, p.Id AS PeriodicalId 
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.Periodicalid
                                LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
                            WHERE p.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND a.sequenceNumber = @SequenceNumber 
                                AND ac.Language = @Language";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = content.PeriodicalId,
                VolumeNumber = content.VolumeNumber,
                IssueNumber = content.IssueNumber,
                SequenceNumber = content.SequenceNumber,
                Language = content.Language
            }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<IssueArticleContentModel>(command);
        }
    }

    public async Task<IssueArticleContentModel> AddIssueArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO  IssueArticleContent (ArticleId, `Language`, `Text`, `FileId`)
                            SELECT a.Id, @Language, @Text, @FileId 
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId =  @LibraryId
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND a.SequenceNumber = @SequenceNumber
                            LIMIT 1";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = content.PeriodicalId,
                VolumeNumber = content.VolumeNumber,
                IssueNumber = content.IssueNumber,
                SequenceNumber = content.SequenceNumber,
                Language = content.Language,
                FileId = content.FileId,
                Text = content.Text
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
            return await GetIssueArticleContentById(libraryId, content.PeriodicalId, content.VolumeNumber, content.IssueNumber, content.SequenceNumber, content.Language, cancellationToken);
        }
    }

    public async Task<IssueArticleContentModel> UpdateIssueArticleContent(int libraryId, IssueArticleContentModel content, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssueArticleContent ac
                                INNER JOIN IssueArticle a ON a.Id = ac.Articleid
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            SET FileId = @FileId,
                                ac.Language = @Language
                            WHERE p.LibraryId =  @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND i.Id = @id";
                                
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                Id = content.Id,
                PeriodicalId = content.PeriodicalId,
                VolumeNumber = content.VolumeNumber,
                IssueNumber = content.IssueNumber,
                SequenceNumber = content.SequenceNumber,
                Language = content.Language,
                FileId = content.FileId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }

        return await GetIssueArticleContentById(libraryId, content.PeriodicalId, content.VolumeNumber, content.IssueNumber, content.SequenceNumber, content.Language, cancellationToken);
    }

    public async Task DeleteIssueArticleContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE ac
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.Periodicalid
                                LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
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

    public async Task<IssueArticleModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            SET a.WriterAccountId = @WriterAccountId, 
                                a.WriterAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE p.LibraryId = @LibraryId 
                                AND p.Id = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber
                                AND a.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId, 
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber, 
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber,
                WriterAccountId = accountId,
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetIssueArticle(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
        }
    }

    public async Task<IssueArticleModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            SET a.ReviewerAccountId = @ReviewerAccountId, 
                                a.ReviewerAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE p.LibraryId = @LibraryId 
                                AND p.Id = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber
                                AND a.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId, 
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber, 
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber,
                ReviewerAccountId = accountId,
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetIssueArticle(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
        }
    }

    private async Task<IssueArticleModel> GetIssueArticleById(long articleId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            IssueArticleModel article = null;
            var sql = @"SELECT a.*, ac.*, au.*,
                            aw.Name As WriterAccountName, 
                            ar.Name As ReviewerAccountName
                            FROM IssueArticle a
                                INNER JOIN Issue i ON i.Id = a.IssueId
                                LEFT OUTER JOIN IssueArticleAuthor iaa ON iaa.IssueArticleId = a.Id
                                LEFT OUTER JOIN Author au On iaa.AuthorId = au.Id
                                LEFT OUTER JOIN IssueArticleContent ac ON a.Id = ac.ArticleId
                                LEFT OUTER JOIN Accounts aw ON aw.Id = a.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = a.ReviewerAccountId
                            WHERE a.Id = @Id";
            var command = new CommandDefinition(sql, new
            {
                Id = articleId
            }, cancellationToken: cancellationToken);
            await connection.QueryAsync<IssueArticleModel, IssueArticleContentModel, AuthorModel, string, string, IssueArticleModel>(command, 
                (a, ac, iaa, w, r) =>
            {
                if (article == null)
                {
                    a.WriterAccountName = w;
                    a.ReviewerAccountName = r;
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
                
                if (iaa != null)
                {
                    var author = article.Authors.SingleOrDefault(x => x.Id == iaa.Id);
                    if (author == null)
                    {
                        article.Authors.Add(iaa);
                    }
                }

                return article;
            }, splitOn: "Id, WriterAccountName,ReviewerAccountName");

            return article;
        }
    }

    private async Task ReorderArticles(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT a.Id, row_number() OVER (ORDER BY a.SequenceNumber) AS 'SequenceNumber'
                            FROM IssueArticle a
                            INNER JOIN Issue i On i.Id = a.IssueId
                            INNER JOIN Periodical p On p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId 
                                AND p.Id = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                            ORDER BY a.SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber
            }, cancellationToken: cancellationToken);
            var newOrder = await connection.QueryAsync(command);

            var sql2 = @"UPDATE IssueArticle
                            SET SequenceNumber = @SequenceNumber
                            WHERE Id = @Id";
            var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command2);
        }
    }

    public async Task UpdateArticleSequence(int libraryId, int periodicalId, int volumeNumber, int issueNumber, IEnumerable<IssueArticleModel> articles, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssueArticle a
                                INNER JOIN Issue i On i.Id = a.IssueId
                                INNER JOIN Periodical p On p.Id = i.PeriodicalId 
                            SET a.SequenceNumber = @SequenceNumber
                            WHERE p.LibraryId = @LibraryId 
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
