﻿using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class IssuePageRepository : IIssuePageRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public IssuePageRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IssuePageModel> AddPage(int libraryId, IssuePageModel issuePage, CancellationToken cancellationToken)
    {
        int pageId;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO IssuePage(IssueId, 
                                SequenceNumber, 
                                FileId, 
                                ImageId,
                                ArticleId,
                                `Status`,
                                WriterAccountId,
                                WriterAssignTimeStamp,
                                ReviewerAccountId,
                                ReviewerAssignTimeStamp)
                            SELECT Id, @SequenceNumber, 
                                @FileId,
                                @ImageId,
                                @ArticleId,
                                @Status,
                                @WriterAccountId,
                                @WriterAssignTimeStamp,
                                @ReviewerAccountId,
                                @ReviewerAssignTimeStamp
                            FROM `Issue` 
                            WHERE PeriodicalId = @PeriodicalId 
                                AND Volumenumber = @VolumeNumber 
                                AND IssueNumber = @IssueNumber;
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, new
            {
                PeriodicalId = issuePage.PeriodicalId,
                VolumeNumber = issuePage.VolumeNumber,
                IssueNumber = issuePage.IssueNumber,
                SequenceNumber = issuePage.SequenceNumber,
                FileId = issuePage.FileId,
                ImageId = issuePage.ImageId,
                Status = issuePage.Status,
                ArticleId = issuePage.ArticleId,
                WriterAccountId = issuePage.WriterAccountId,
                WriterAssignTimeStamp = issuePage.WriterAssignTimeStamp,
                ReviewerAccountId = issuePage.ReviewerAccountId,
                ReviewerAssignTimeStamp = issuePage.ReviewerAssignTimeStamp
                
            }, cancellationToken: cancellationToken);
            pageId = await connection.ExecuteScalarAsync<int>(command);
        }

        await ReorderPages(libraryId, issuePage.PeriodicalId, issuePage.VolumeNumber, issuePage.IssueNumber, cancellationToken);
        return await GetPageById(pageId, cancellationToken);
    }

    public async Task<IssuePageModel> GetPageBySequenceNumber(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber, p.SequenceNumber, p.FileId, p.Status, p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp, 
                                p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp, 
                                f.Id As ImageId, f.FilePath AS ImageUrl, ia.Id As ArticleId, ia.Title As ArticleName
                            FROM IssuePage AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN IssueArticle ia ON ia.Id = p.ArticleId
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                LEFT OUTER JOIN Accounts a ON a.Id = p.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PeriodicalId = periodicalId,
                                                    VolumeNumber = volumeNumber,
                                                    IssueNumber = issueNumber,
                                                    SequenceNumber = sequenceNumber
                                                },
                                                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<IssuePageModel>(command);
        }
    }

    public async Task DeletePage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE p 
                            FROM IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
            await ReorderPages(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
        }
    }

    public async Task<IssuePageModel> UpdatePage(int libraryId, IssuePageModel issuePage, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            SET p.FileId = @FileId, 
                                p.ImageId = @ImageId, 
                                p.`Status` = @Status,
                                p.ArticleId = @ArticleId,
                                p.WriterAccountId = @WriterAccountId,
                                p.WriterAssignTimeStamp = @WriterAssignTimeStamp,
                                p.ReviewerAccountId = @ReviewerAccountId,
                                p.ReviewerAssignTimeStamp = @ReviewerAssignTimeStamp
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                FileId = issuePage.FileId,
                ImageId = issuePage.ImageId,
                PeriodicalId = issuePage.PeriodicalId,
                VolumeNumber = issuePage.VolumeNumber,
                IssueNumber = issuePage.IssueNumber,
                SequenceNumber = issuePage.SequenceNumber,
                Status = issuePage.Status,
                WriterAccountId = issuePage.WriterAccountId,
                WriterAssignTimeStamp = issuePage.WriterAssignTimeStamp,
                ReviewerAccountId = issuePage.ReviewerAccountId,
                ReviewerAssignTimeStamp = issuePage.ReviewerAssignTimeStamp,
                ArticleId = issuePage.ArticleId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }

        return await GetPageBySequenceNumber(libraryId, issuePage.PeriodicalId, issuePage.VolumeNumber, issuePage.IssueNumber, issuePage.SequenceNumber, cancellationToken);
    }

    public async Task<IssuePageModel> UpdatePageImage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, long imageId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            SET p.ImageId = @ImageId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber,
                ImageId = imageId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetPageBySequenceNumber(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
        }
    }

    public async Task DeletePageImage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            SET p.ImageId = NULL
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
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

    public async Task<int> GetPageCount(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())

        {
            var sql = @"SELECT COUNT(p.Id)
                            FROM IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber
            }, cancellationToken: cancellationToken);
            return await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task<Page<IssuePageModel>> GetPagesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int pageNumber, int pageSize, EditingStatus status, AssignmentFilter assignmentFilter, AssignmentFilter reviewerAssignmentFilter, int? assignedTo, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber,
                                   p.SequenceNumber, p.Status, 
                                   p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp,
                                   p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp,
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.FileId, p.ArticleId, ia.Title As ArticleName
                            FROM IssuePage AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN IssueArticle ia ON ia.Id = p.ArticleId
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                LEFT OUTER JOIN Accounts a ON a.Id = p.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND (@Status = 0 OR p.Status = @Status )
                                AND (
                                    ( @AssignmentFilter = 0 ) OR
                                    ( @AssignmentFilter = 1 AND p.WriterAccountId IS NOT NULL) OR
                                    ( @AssignmentFilter = 2 AND p.WriterAccountId IS NULL) OR
                                    ( (@AssignmentFilter = 3  OR @AssignmentFilter = 4) AND p.WriterAccountId = @AccountId )
                                )
                                AND (
                                    ( @ReviewerAssignmentFilter = 0 ) OR
                                    ( @ReviewerAssignmentFilter = 1 AND p.ReviewerAccountId IS NOT NULL) OR
                                    ( @ReviewerAssignmentFilter = 2 AND p.ReviewerAccountId IS NULL) OR
                                    ( (@ReviewerAssignmentFilter = 3  OR @ReviewerAssignmentFilter = 4) AND p.ReviewerAccountId = @AccountId )
                                )
                            ORDER BY p.SequenceNumber
                            LIMIT @PageSize
                            OFFSET @Offset";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PeriodicalId = periodicalId,
                                                    VolumeNumber = volumeNumber,
                                                    IssueNumber = issueNumber,
                                                    Status = status,
                                                    PageSize = pageSize,
                                                    Offset = pageSize * (pageNumber - 1),
                                                    AssignmentFilter = assignmentFilter,
                                                    ReviewerAssignmentFilter = reviewerAssignmentFilter,
                                                    AccountId = assignedTo
                                                },
                                                cancellationToken: cancellationToken);

            var pages = await connection.QueryAsync<IssuePageModel>(command);

            var sqlCount = @"SELECT Count(*)
                                FROM IssuePage p 
                                    INNER JOIN Issue i ON i.Id = p.IssueId
                                    INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                WHERE pr.LibraryId = @LibraryId 
                                    AND i.PeriodicalId = @PeriodicalId 
                                    AND i.VolumeNumber = @VolumeNumber 
                                    AND i.IssueNumber = @IssueNumber
                                    AND (@Status = 0 OR p.Status = @Status )
                                    AND (
                                        ( @AssignmentFilter = 0 ) OR
                                        ( @AssignmentFilter = 1 AND p.WriterAccountId IS NOT NULL) OR
                                        ( @AssignmentFilter = 2 AND p.WriterAccountId IS NULL) OR
                                        ( (@AssignmentFilter = 3  OR @AssignmentFilter = 4) AND p.WriterAccountId = @AccountId)
                                    )
                                    AND (
                                    ( @ReviewerAssignmentFilter = 0 ) OR
                                    ( @ReviewerAssignmentFilter = 1 AND p.ReviewerAccountId IS NOT NULL) OR
                                    ( @ReviewerAssignmentFilter = 2 AND p.ReviewerAccountId IS NULL) OR
                                    ( (@ReviewerAssignmentFilter = 3  OR @ReviewerAssignmentFilter = 4) AND p.ReviewerAccountId = @AccountId)
                                )";
            var commandCount = new CommandDefinition(sqlCount, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                Status = status,
                AssignmentFilter = assignmentFilter,
                ReviewerAssignmentFilter = reviewerAssignmentFilter,
                AccountId = assignedTo
            },
                cancellationToken: cancellationToken);

            var pagesCount = await connection.QuerySingleAsync<int>(commandCount);
            return new Page<IssuePageModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagesCount,
                Data = pages
            };
        }
    }

    public async Task<IssuePageModel> UpdateWriterAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            SET p.WriterAccountId = @WriterAccountId, p.WriterAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber,
                WriterAccountId = assignedAccountId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetPageBySequenceNumber(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
        }
    }

    public async Task<IssuePageModel> UpdateReviewerAssignment(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int? assignedAccountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            SET p.ReviewerAccountId = @ReviewerAccountId, p.ReviewerAssignTimeStamp = UTC_TIMESTAMP()
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                SequenceNumber = sequenceNumber,
                ReviewerAccountId = assignedAccountId
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);

            return await GetPageBySequenceNumber(libraryId, periodicalId, volumeNumber, issueNumber, sequenceNumber, cancellationToken);
        }
    }

    public async Task<int> GetLastPageNumberForIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT Max(p.SequenceNumber)
                            FROM IssuePage AS p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr ON pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PeriodicalId = periodicalId,
                                                    VolumeNumber = volumeNumber,
                                                    IssueNumber = issueNumber
                                                },
                                                cancellationToken: cancellationToken);

            return await connection.ExecuteScalarAsync<int>(command);
        }
    }

    public async Task<IEnumerable<IssuePageModel>> GetAllPagesByIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber, p.SequenceNumber, p.Status, 
                                p.WriterAccountId, p.WriterAssignTimeStamp, 
                                p.ReviewerAccountId, p.ReviewerAssignTimeStamp, 
                                f.Id As ImageId, f.FilePath AS ImageUrl, p.FileId, p.ArticleId, ia.Title As ArticleName
                            FROM IssuePage AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN `Article` ia ON ia.Id = p.ArticleId
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber
                            ORDER BY p.SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber
            }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<IssuePageModel>(command);
        }
    }

    public async Task<Page<IssuePageModel>> GetPagesByUser(int libraryId, int accountId, EditingStatus statusFilter, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber, p.SequenceNumber, p.Status, 
                                   p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp,
                                   p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp,
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.FileId, p.ArticleId, ia.Title As ArticleName
                            FROM IssuePage AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN IssueArticle ia ON ia.Id = p.ArticleId
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                LEFT OUTER JOIN Accounts a ON a.Id = p.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND p.ReviewerAccountId = @AccountId OR p.WriterAccountId = @AccountId
                                AND (@Status = -1 OR p.Status = @Status )
                            ORDER BY i.Id, p.SequenceNumber
                            LIMIT @PageSize OFFSET @Offset";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    Status = statusFilter,
                                                    PageSize = pageSize,
                                                    Offset = pageSize * (pageNumber - 1),
                                                    AccountId = accountId
                                                },
                                                cancellationToken: cancellationToken);

            var pages = await connection.QueryAsync<IssuePageModel>(command);

            var sqlCount = @"SELECT Count(*)
                                FROM IssuePage p 
                                    INNER JOIN Issue i ON i.Id = p.IssueId
                                    INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                WHERE pr.LibraryId = @LibraryId
                                    AND p.ReviewerAccountId = @AccountId
                                    OR p.WriterAccountId = @AccountId
                                    AND (@Status = -1 OR p.Status = @Status )";
            var commandCount = new CommandDefinition(sqlCount, new
            {
                LibraryId = libraryId,
                Status = statusFilter,
                AccountId = accountId
            },
                cancellationToken: cancellationToken);

            var pagesCount = await connection.QuerySingleAsync<int>(commandCount);
            return new Page<IssuePageModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = pagesCount,
                Data = pages
            };
        }
    }

    public async Task<IEnumerable<IssuePageModel>> GetPagesByIssueArticle(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long articleId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber,
                                   p.SequenceNumber, p.Status, 
                                   p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp,
                                   p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp,
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.FileId, p.ArticleId, ia.Title As ArticleName
                            FROM IssuePage AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN IssueArticle ia ON ia.Id = p.ArticleId
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                LEFT OUTER JOIN Accounts a ON a.Id = p.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber   
                                AND ia.Id = @ArticleId
                            ORDER BY p.SequenceNumber";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PeriodicalId = periodicalId,
                                                    VolumeNumber = volumeNumber,
                                                    IssueNumber = issueNumber,
                                                    ArticleId= articleId
                                                },
                                                cancellationToken: cancellationToken);

            return await connection.QueryAsync<IssuePageModel>(command);
        }
    }
    public async Task ReorderPages(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT p.Id, row_number() OVER (ORDER BY p.SequenceNumber) AS 'SequenceNumber'
                            FROM IssuePage p
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber
                            ORDER BY p.SequenceNumber";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
            }, cancellationToken: cancellationToken);
            var newOrder = await connection.QueryAsync(command);

            var sql2 = @"UPDATE IssuePage
                            SET SequenceNumber = @SequenceNumber
                            WHERE Id = @Id";
            var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command2);
        }
    }

    public async Task UpdatePageSequenceNumber(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int oldSequenceNumber, int newSequenceNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {

            var issueId = await connection.ExecuteScalarAsync<long>(
                new CommandDefinition(
                    @"SELECT Id FROM Issue  
                        WHERE VolumeNumber = @VolumeNumber
                            AND IssueNumber = @IssueNumber",
                    new
                    {
                        VolumeNumber = volumeNumber,
                        IssueNumber = issueNumber,
                    }, cancellationToken: cancellationToken));
            var existingId = await connection.ExecuteScalarAsync<long>(
                new CommandDefinition(
                    @"SELECT Id FROM IssuePage 
                        WHERE IssueId = @IssueId AND SequenceNumber = @oldPosition",
                    new
                    {
                        IssueId = issueId,
                        oldPosition = oldSequenceNumber,
                    }, cancellationToken: cancellationToken));
            var maxPosition = await connection.ExecuteScalarAsync<long>(
                new CommandDefinition(
                    @"SELECT MAX(SequenceNumber) FROM IssuePage
                        WHERE IssueId = @IssueId",
                    new
                    {
                        IssueId = issueId
                    }, cancellationToken: cancellationToken));
            var newPosition = newSequenceNumber < 1 ? 1 :  newSequenceNumber > maxPosition ? maxPosition : newSequenceNumber;
            
            var sql = @"UPDATE IssuePage 
                        SET SequenceNumber = CASE
                            WHEN Id = @Id THEN @newPosition
                            WHEN @oldPosition < @newPosition THEN SequenceNumber - 1
                            WHEN @oldPosition > @newPosition THEN SequenceNumber + 1
                        END
                        WHERE IssueId = @IssueId AND SequenceNumber BETWEEN
                            LEAST(@oldPosition, @newPosition) AND
                            GREATEST(@oldPosition, @newPosition);";
            var command = new CommandDefinition(sql, new
            {
                IssueId = issueId,
                oldPosition = oldSequenceNumber,
                newPosition = newPosition,
                Id = existingId
            }, cancellationToken: cancellationToken);

            await connection.ExecuteAsync(command);
        }
    }

    public async Task<IEnumerable<UserPageSummaryItem>> GetUserPageSummary(int libraryId, int accountId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT 1 As Status, Count(p.Id) As Count 
                            FROM IssuePage p 
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND p.`Status` = 1 
                                AND p.WriterAccountId = @AccountId
                        UNION
                            SELECT 2 As Status, Count(p.Id) As Count 
                            FROM IssuePage p 
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND p.`Status` = 2 
                                AND p.WriterAccountId = @AccountId
                        UNION
                            SELECT 3 As Status, Count(p.Id) As Count 
                            FROM IssuePage p 
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId
                                AND p.`Status` = 3
                                AND p.ReviewerAccountId = @AccountId
                        UNION
                            SELECT 4 As Status, Count(p.Id) As Count 
                            FROM IssuePage p 
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId
                                AND p.`Status` = 4
                                AND p.ReviewerAccountId = @AccountId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                AccountId = accountId
            }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<UserPageSummaryItem>(command);
        }
    }

    public async Task<IssuePageModel> GetPageById(int pageId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber, p.SequenceNumber, p.FileId, p.Status, p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp, 
                                p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp, 
                                f.Id As ImageId, f.FilePath AS ImageUrl, ia.Id As ArticleId, ia.Title As ArticleName
                            FROM IssuePage AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN IssueArticle ia ON ia.Id = p.ArticleId
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                LEFT OUTER JOIN Accounts a ON a.Id = p.WriterAccountId
                                LEFT OUTER JOIN Accounts ar ON ar.Id = p.ReviewerAccountId
                            WHERE p.Id = @Id";
            var command = new CommandDefinition(sql, new { Id = pageId }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<IssuePageModel>(command);
        }
    }
}
