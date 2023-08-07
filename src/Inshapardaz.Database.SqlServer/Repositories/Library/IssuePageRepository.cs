using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class IssuePageRepository : IIssuePageRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public IssuePageRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<IssuePageModel> AddPage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string text, int? imageId, int? articleId, CancellationToken cancellationToken)
        {
            int pageId;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO IssuePage(IssueId, SequenceNumber, Text, ImageId, ArticleId)
                            OUTPUT Inserted.Id
                            SELECT Id, @SequenceNumber, @Text, @ImageId, @ArticleId
                            FROM Issue WHERE PeriodicalId = @PeriodicalId AND Volumenumber = @VolumeNumber AND IssueNumber = @IssueNumber;";
                var command = new CommandDefinition(sql, new {
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber, 
                    SequenceNumber = sequenceNumber, 
                    Text = text, 
                    ImageId = imageId,
                    ArticleId = articleId 
                }, cancellationToken: cancellationToken);
                pageId = await connection.ExecuteScalarAsync<int>(command);
            }

            await ReorderPages(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
            return await GetPageById(pageId, cancellationToken);
        }

        public async Task<IssuePageModel> GetPageBySequenceNumber(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber, p.SequenceNumber, p.Text, p.Status, p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp, 
                            p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp, 
                            f.Id As ImageId, f.FilePath AS ImageUrl, ia.Id As ArticleId, ia.Title As ArticleTitle
                            FROM IssuePage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [IssueArticle] ia ON ia.Id = p.ArticleId
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, 
                                                          PeriodicalId = periodicalId, 
                                                          VolumeNumber = volumeNumber, 
                                                          IssueNumber= issueNumber, 
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
                var sql = @"Delete p From IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                             Where pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
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

        public async Task<IssuePageModel> UpdatePage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, string text, int? imageId, int? articleId, EditingStatus status, CancellationToken cancellationToken)
        {
            int pageId;

            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET p.Text = @Text, 
                                p.ImageId = @ImageId, 
                                Status = @Status,
                                ArticleId = @ArticleId
                            OUTPUT Inserted.Id
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    Text = text,
                    ImageId = imageId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    SequenceNumber = sequenceNumber,
                    Status = status,
                    ArticleId = articleId
                }, cancellationToken: cancellationToken);
                pageId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetPageById(pageId, cancellationToken);
            
        }

        public async Task<IssuePageModel> UpdatePageImage(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int sequenceNumber, int imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Update p
                            SET p.ImageId = @ImageId
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
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
                var sql = @"Update p
                            SET ImageId = NULL
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
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
                var sql = @"SELECT COUNT(p.*)
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
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
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ArticleId, ia.Title As ArticleTitle
                            FROM IssuePage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [IssueArticle] ia ON ia.Id = p.ArticleId
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND (@Status = -1 OR p.Status = @Status )
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
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new
                                                    {
                                                        LibraryId = libraryId,
                                                        PeriodicalId = periodicalId,
                                                        VolumeNumber = volumeNumber,
                                                        IssueNumber = issueNumber,
                                                        Status = status,
                                                        PageSize = pageSize,
                                                        PageNumber = pageNumber,
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
                                    AND (@Status = -1 OR p.Status = @Status )
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
                var sql = @"Update p
                            SET p.WriterAccountId = @WriterAccountId, p.WriterAssignTimeStamp = GETUTCDATE()
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
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
                var sql = @"Update p
                            SET p.ReviewerAccountId = @ReviewerAccountId, p.ReviewerAssignTimeStamp = GETUTCDATE()
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND p.SequenceNumber = @SequenceNumber";
                var command = new CommandDefinition(sql, new {
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
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber";
                var command = new CommandDefinition(sql,
                                                    new {
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
                            f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ArticleId, ia.Title As ArticleTitle
                            FROM IssuePage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [Article] ia ON ia.Id = p.ArticleId
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber
                            ORDER BY p.SequenceNumber";
                var command = new CommandDefinition(sql, new {
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
                                   f.Id As ImageId, f.FilePath AS ImageUrl, p.Text, p.ArticleId, ia.Title As ArticleTitle
                            FROM IssuePage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [IssueArticle] ia ON ia.Id = p.ArticleId
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            WHERE pr.LibraryId = @LibraryId 
                                AND p.ReviewerAccountId = @AccountId OR p.WriterAccountId = @AccountId
                                AND (@Status = -1 OR p.Status = @Status )
                            ORDER BY i.IssueId, p.SequenceNumber
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new
                                                    {
                                                        LibraryId = libraryId,
                                                        Status = statusFilter,
                                                        PageSize = pageSize,
                                                        PageNumber = pageNumber,
                                                        AccountId = accountId
                                                    },
                                                    cancellationToken: cancellationToken);

                var pages = await connection.QueryAsync<IssuePageModel>(command);

                var sqlCount = @"SELECT Count(*)
                                FROM IssuePage p 
                                INNER JOIN Issue i ON i.Id = p.IssueId
                                INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                                WHERE pr.LibraryId = @LibraryId
                                AND p.ReviewerAccountId = @AccountId OR p.WriterAccountId = @AccountId
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

        public async Task ReorderPages(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.Id, row_number() OVER (ORDER BY p.SequenceNumber) as 'SequenceNumber'
                            From IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber
                            Order By p.SequenceNumber";
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
                            Where Id = @Id";
                var command2 = new CommandDefinition(sql2, newOrder, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command2);
            }
        }

        public async Task UpdatePageSequenceNumber(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int oldSequenceNumber, int newSequenceNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {

                var sql = @"DECLARE @maxPosition INT;
                            DECLARE @Id INT;

                            SELECT @Id = ip.Id
                            FROM IssuePage ip
                            INNER JOIN Issue i ON i.Id = ip.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND ip.SequenceNumber = @oldPosition;

                            SELECT @maxPosition = MAX(p.SequenceNumber) 
                            FROM IssuePage p
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 

                            IF (@newPosition < 1)
                             SET @newPosition = 1
 
                            IF (@newPosition > @maxPosition)
                             SET @newPosition = @maxPosition

                            UPDATE ip 
                            SET ip.SequenceNumber = CASE
                                WHEN ip.Id = @Id THEN @newPosition
                                WHEN @oldPosition < @newPosition THEN ip.SequenceNumber - 1
                                WHEN @oldPosition > @newPosition THEN ip.SequenceNumber + 1
                            END
                            FROM IssuePage ip
                            INNER JOIN Issue i ON i.Id = ip.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND ip.SequenceNumber BETWEEN
                                    CASE WHEN @oldPosition < @newPosition THEN @oldPosition ELSE @newPosition END AND
                                    CASE WHEN @oldPosition > @newPosition THEN @oldPosition ELSE @newPosition END;
";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    oldPosition = oldSequenceNumber,
                    newPosition = newSequenceNumber,

                }, cancellationToken: cancellationToken);
                
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<UserPageSummaryItem>> GetUserPageSummary(int libraryId, int accountId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT 1 As Status, Count(p.Id) As Count 
                            FROM IsseuPage p 
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND p.[Status] = 1 
                                AND p.WriterAccountId = @AccountId
                        UNION
                            SELECT 2 As Status, Count(p.Id) As Count 
                            FROM IsseuPage p 
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId 
                                AND p.[Status] = 2 
                                AND p.WriterAccountId = @AccountId
                        UNION
                            SELECT 3 As Status, Count(p.Id) As Count 
                            FROM IsseuPage p 
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId
                                AND p.[Status] = 3
                                AND p.ReviewerAccountId = @AccountId
                        UNION
                            SELECT 4 As Status, Count(p.Id) As Count 
                            FROM IsseuPage p 
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            WHERE pr.LibraryId = @LibraryId
                                AND p.[Status] = 4
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
                var sql = @"SELECT i.PeriodicalId, i.VolumeNumber, i.IssueNumber, p.SequenceNumber, p.Text, p.Status, p.WriterAccountId, a.Name As WriterAccountName, p.WriterAssignTimeStamp, 
                            p.ReviewerAccountId, ar.Name As ReviewerAccountName, p.ReviewerAssignTimeStamp, 
                            f.Id As ImageId, f.FilePath AS ImageUrl, ia.Id As ArticleId, ia.Title As ArticleTitle
                            FROM IssuePage AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            LEFT OUTER JOIN [IssueArticle] ia ON ia.Id = p.ArticleId
                            INNER JOIN Issue i ON i.Id = p.IssueId
                            INNER JOIN Periodical pr on pr.Id = i.PeriodicalId
                            LEFT OUTER JOIN [Accounts] a ON a.Id = p.WriterAccountId
                            LEFT OUTER JOIN [Accounts] ar ON ar.Id = p.ReviewerAccountId
                            WHERE p.Id = @Id";
                var command = new CommandDefinition(sql, new { Id = pageId },cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<IssuePageModel>(command);
            }
        }
    }
}
