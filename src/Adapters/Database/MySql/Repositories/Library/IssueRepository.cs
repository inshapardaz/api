using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library;

public class IssueRepository : IIssueRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public IssueRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<Page<IssueModel>> GetIssues(int libraryId, int periodicalId, int pageNumber, int pageSize, IssueFilter filter, IssueSortByType sortBy, SortDirection sortDirection, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sortByQuery = $"i.{GetSortByQuery(sortBy)}";
            var direction = sortDirection == SortDirection.Descending ? "DESC" : "ASC";

            var sql = @"SELECT i.*, f.FilePath as ImageUrl,
                                (SELECT COUNT(*) FROM IssueArticle WHERE IssueId = i.id) As ArticleCount,
                                (SELECT COUNT(*) FROM IssuePage WHERE IssueId = i.id) As `PageCount`,
                                p.*, t.*
                            FROM Issue AS i
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                LEFT OUTER JOIN `File` f ON f.Id = i.ImageId
                                LEFT OUTER JOIN IssueTag it ON i.Id = it.IssueId
                                LEFT OUTER JOIN Tag t ON t.Id = it.TagId
                            WHERE p.LibraryId = @LibraryId
                                AND p.Id = @PeriodicalId
                                AND (@Year IS NULL OR YEAR(i.IssueDate) = @Year)
                                AND (@VolumeNumber IS NULL OR VolumeNumber = @VolumeNumber) " +
                            $" ORDER BY {sortByQuery} {direction} " +
                        @"LIMIT @PageSize OFFSET @Offset";
            var command = new CommandDefinition(sql,
                                                new
                                                {
                                                    LibraryId = libraryId,
                                                    PeriodicalId = periodicalId,
                                                    PageSize = pageSize,
                                                    Offset = pageSize * (pageNumber - 1),
                                                    Year = filter.Year.HasValue ? filter.Year : null,
                                                    VolumeNumber = filter.VolumeNumber.HasValue ? filter.VolumeNumber : null
                                                },

                                                cancellationToken: cancellationToken);

            var issues = new Dictionary<long, IssueModel>();
            await connection.QueryAsync<IssueModel, PeriodicalModel, TagModel, IssueModel>(command, (i, periodical, tag) =>
            {
                if (!issues.TryGetValue(i.Id, out IssueModel issue))
                    issues.Add(i.Id, issue = i);
                
                issue.Periodical = periodical;
                
                if (tag != null && !issue.Tags.Any(x => x.Id == tag.Id))
                {
                    issue.Tags.Add(tag);
                }
                return i;
            });

            var totalCountSql = @"SELECT COUNT(*) 
                        FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                        WHERE p.LibraryId = @LibraryId 
                            AND p.Id = @PeriodicalId";
            var totalCount = await connection.QuerySingleAsync<int>(new CommandDefinition(totalCountSql, new { LibraryId = libraryId, PeriodicalId = periodicalId }, cancellationToken: cancellationToken));

            return new Page<IssueModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = issues.Values
            };
        }
    }

    public async Task<IEnumerable<(int Year, int count)>> GetIssuesYear(int libraryId, int periodicalId, AssignmentStatus assignmentStatus, SortDirection sortDirection, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var direction = sortDirection == SortDirection.Descending ? "DESC" : "ASC";

            var sql = @"SELECT YEAR(i.IssueDate) `Year`, COUNT(*)
                            FROM Issue AS i
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId
                                AND p.Id = @PeriodicalId 
                            GROUP BY YEAR(i.IssueDate) " +
                    $" ORDER BY `Year` {direction}";
            var command = new CommandDefinition(sql,
                    new
                    {
                        LibraryId = libraryId,
                        PeriodicalId = periodicalId
                    },
                    cancellationToken: cancellationToken);

            return await connection.QueryAsync<(int Year, int count)>(command);
        }
    }

    public async Task<IssueModel> GetIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.*, p.*, f.FilePath as ImageUrl,
                                (SELECT COUNT(*) FROM IssueArticle WHERE IssueId = i.id) As ArticleCount,
                                (SELECT COUNT(*) FROM IssuePage WHERE IssueId = i.id) As `PageCount`,
                                 t.*
                            FROM Issue as i
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                LEFT OUTER JOIN `File` f ON f.Id = i.ImageId
                                LEFT OUTER JOIN IssueTag it ON i.Id = it.IssueId
                                LEFT OUTER JOIN Tag t ON t.Id = it.TagId
                            WHERE p.LibraryId = @LibraryId
                                AND p.Id = @PeriodicalId
                                AND i.IssueNumber = @IssueNumber
                                AND i.VolumeNumber = @VolumeNumber";
            var parameter = new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                IssueNumber = issueNumber,
                VolumeNumber = volumeNumber
            };
            var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);

            IssueModel issue = null;
            await connection.QueryAsync<IssueModel, PeriodicalModel, string, long, long, TagModel, IssueModel>(command, (i, p, iUrl, ac, pc, t) =>
            {
                if (issue == null)
                {
                    issue = i;
                }

                issue.Periodical = p;
                issue.ArticleCount = (int)ac;
                issue.PageCount = (int)pc;
                issue.ImageUrl = iUrl;
                
                if (t != null && i.Tags.All(x => x.Id != t.Id))
                {
                    issue.Tags.Add(t);
                }
                return issue;
            }, splitOn: "Id, ImageUrl, ArticleCount, PageCount, Id");

            return issue;
        }
    }

    public async Task<IssueModel> AddIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken)
    {
        int id;
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO Issue (PeriodicalId, Volumenumber, IssueNumber, IsPublic, ImageId, IssueDate, Status) 
                        VALUES (@PeriodicalId, @Volumenumber, @IssueNumber, @IsPublic, @ImageId, @IssueDate, @Status);
                        SELECT LAST_INSERT_ID();";
            var parameter = new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                Volumenumber = issue.VolumeNumber,
                IssueNumber = issue.IssueNumber,
                IssueDate = issue.IssueDate,
                IsPublic = issue.IsPublic,
                ImageId = issue.ImageId,
                Status = issue.Status
            };
            var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
            
            if (issue.Tags != null && issue.Tags.Any())
            {
                foreach (var tag in issue.Tags)
                {
                    var tagId = await connection.ExecuteScalarAsync<int>(
                        new CommandDefinition(
                            @"INSERT INTO Tag (Name, LibraryId) 
                              VALUES (@Name, @LibraryId) 
                              ON DUPLICATE KEY UPDATE Name=@Name; 
                              SELECT Id FROM Tag WHERE Name = @Name AND  LibraryId = @LibraryId;",
                            new { Name = tag.Name, LibraryId = libraryId },
                            cancellationToken: cancellationToken));
            
                    // Associate tag with periodical
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            "INSERT INTO IssueTag (IssueId, TagId) VALUES (@IssuesId, @TagId);",
                            new { IssuesId = id, TagId = tagId },
                            cancellationToken: cancellationToken));
                }
            }
        }

        return await GetIssueById(libraryId, periodicalId, id, cancellationToken);
    }

    public async Task UpdateIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE Issue
                            SET PeriodicalId = @PeriodicalId, 
                                Volumenumber = @Volumenumber, 
                                IssueNumber = @IssueNumber, 
                                IssueDate = @IssueDate, 
                                IsPublic = @IsPublic, 
                                ImageId = @ImageId,
                                Status = @Status
                            WHERE Id = @Id";
            var parameter = new
            {
                Id = issue.Id,
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                Volumenumber = issue.VolumeNumber,
                IssueNumber = issue.IssueNumber,
                IssueDate = issue.IssueDate,
                IsPublic = issue.IsPublic,
                ImageId = issue.ImageId,
                Status = issue.Status,
            };
            var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);
            
            await connection.ExecuteAsync(new CommandDefinition(
                "DELETE FROM IssueTag WHERE IssueId = @IssueId",
                new { IssueId = issue.Id },
                cancellationToken: cancellationToken));
            
            if (issue.Tags != null && issue.Tags.Any())
            {
                foreach (var tag in issue.Tags)
                {
                    var tagId = await connection.ExecuteScalarAsync<int>(
                        new CommandDefinition(
                            @"INSERT INTO Tag (Name, LibraryId) 
                              VALUES (@Name, @LibraryId) 
                              ON DUPLICATE KEY UPDATE Name=@Name; 
                              SELECT Id FROM Tag WHERE Name = @Name AND  LibraryId = @LibraryId;",
                            new { Name = tag.Name, LibraryId = libraryId },
                            cancellationToken: cancellationToken));
            
                    // Associate tag with issue
                    await connection.ExecuteAsync(
                        new CommandDefinition(
                            "INSERT INTO IssueTag (IssueId, TagId) VALUES (@IssueId, @TagId);",
                            new { IssueId = issue.Id, TagId = tagId },
                            cancellationToken: cancellationToken));
                }
            }
        }
    }

    public async Task DeleteIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE i 
                            FROM Issue i
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId 
                                AND p.Id = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, PeriodicalId = periodicalId, VolumeNumber = volumeNumber, IssueNumber = issueNumber }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    private async Task<IssueModel> GetIssueById(int libraryId, int periodicalId, int issueId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT i.*, p.*, f.FilePath as ImageUrl,
                                (SELECT COUNT(*) FROM IssueArticle WHERE IssueId = i.id) As ArticleCount,
                                (SELECT COUNT(*) FROM IssuePage WHERE IssueId = i.id) As `PageCount`,
                                t.*
                            FROM Issue as i
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                LEFT OUTER JOIN `File` f ON f.Id = i.ImageId
                                LEFT OUTER JOIN IssueTag it ON i.Id = it.IssueId
                                LEFT OUTER JOIN Tag t ON t.Id = it.TagId
                            WHERE p.LibraryId = @LibraryId
                                AND p.Id = @PeriodicalId
                                AND i.Id = @IssueId";
            var parameter = new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                IssueId = issueId
            };
            var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);

            var result = await connection.QueryAsync<IssueModel, PeriodicalModel, string, long, long, TagModel, IssueModel>(command, (i, p, iUrl, ac, pc, t) =>
            {
                i.Periodical = p;
                i.ArticleCount = (int)ac;
                i.PageCount = (int)pc;
                i.ImageUrl = iUrl;
                return i;
            }, splitOn: "Id, ImageUrl, ArticleCount, PageCount, Id");

            return result.SingleOrDefault();
        }
    }

    public async Task<IssueContentModel> AddIssueContent(int libraryId, IssueContentModel model, CancellationToken cancellationToken)
    {
        var issue = await GetIssue(libraryId, model.PeriodicalId, model.VolumeNumber, model.IssueNumber, cancellationToken);
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"INSERT INTO IssueContent (IssueId, FileId, Language, MimeType)
                            VALUES (@IssueId, @FileId, @Language, @MimeType);
                        SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, new { 
                FileId = model.FileId, 
                IssueId = issue.Id, 
                Language = model.Language, 
                MimeType = model.MimeType,
            }, cancellationToken: cancellationToken);
            var id = await connection.ExecuteScalarAsync<long>(command);
            return await GetIssueContentById(connection, id, cancellationToken);
        }
    }

    public async Task<IEnumerable<IssueContentModel>> GetIssueContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT ic.Id, ic.IssueId, i.PeriodicalId, i.VolumeNumber, i.IssueNumber, ic.Language, f.MimeType, f.Id As FileId, f.FileName As FileName, f.FilePath AS ContentUrl
                            FROM IssueContent ic
                                INNER JOIN Issue i ON i.Id = ic.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                INNER JOIN `File` f ON ic.FileId = f.Id
                            WHERE p.LibraryId = @LibraryId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND i.PeriodicalId = @PeriodicalId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber
            }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<IssueContentModel>(command);
        }
    }

    public async Task<IssueContentModel> GetIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"SELECT ic.Id, ic.IssueId, ic.Language, 
                                p.Id As PeriodicalId, i.VolumeNumber As VolumeNumber, i.IssueNumber As IssueNumber,
                                f.MimeType, f.Id As FileId, f.FilePath AS ContentUrl
                            FROM IssueContent ic
                                INNER JOIN Issue i ON i.Id = ic.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                INNER JOIN `File` f ON ic.FileId = f.Id
                            WHERE p.LibraryId = @LibraryId 
                                AND i.VolumeNumber = @VolumeNumber 
                                AND i.IssueNumber = @IssueNumber 
                                AND i.PeriodicalId = @PeriodicalId 
                                AND ic.id = @ContentId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                ContentId = contentId
            }, cancellationToken: cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<IssueContentModel>(command);
        }
    }

    public async Task<IssueContentModel> UpdateIssueContent(int libraryId, IssueContentModel model, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE IssueContent ic
                        INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                        SET ic.Language = @Language,
                            ic.MimeType = @MimeType,
                            ic.FileId = @FileId 
                        WHERE p.LibraryId = @LibraryId 
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND ic.id = @ContentId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = model.PeriodicalId,
                VolumeNumber = model.VolumeNumber,
                IssueNumber = model.IssueNumber,
                ContentId = model.Id,
                Language = model.Language,
                MimeType = model.MimeType,
                FileId = model.FileId,
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }

        return await GetIssueContent(libraryId, model.PeriodicalId, model.VolumeNumber, model.IssueNumber, model.Id, cancellationToken);
    }

    public async Task<IEnumerable<PageSummaryModel>> GetIssuePageSummary(int libraryId, int[] issues, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var bookSummaries = new Dictionary<int, PageSummaryModel>();
            const string sql = @"SELECT ip.IssueId, ip.`Status`, Count(*),
                                (Count(ip.Status)* 100 / (Select Count(*) From IssuePage WHERE IssuePage.IssueId = ip.IssueId)) as Percentage
                                FROM IssuePage ip
                                    INNER Join Issue i ON i.id = ip.IssueId
                                    INNER Join Periodical p ON p.id = i.PeriodicalId
                                WHERE p.LibraryId = @LibraryId
                                    AND i.Id IN @IssueIds
                                    AND i.Status <> 0
                                GROUP By ip.IssueId, ip.`Status`";

            var command = new CommandDefinition(sql, new { LibraryId = libraryId, IssueIds = issues }, cancellationToken: cancellationToken);
            var results = await connection.QueryAsync<(int BookId, EditingStatus Status, int Count, decimal Percentage)>(command);

            foreach (var result in results)
            {
                var pageSummary = new PageStatusSummaryModel { Status = result.Status, Count = result.Count, Percentage = result.Percentage };
                if (!bookSummaries.TryGetValue(result.BookId, out PageSummaryModel bookSummary))
                {
                    bookSummaries.Add(result.BookId, new PageSummaryModel
                    {
                        BookId = result.BookId,
                        Statuses = new List<PageStatusSummaryModel> { pageSummary }
                    });
                }
                else
                {
                    bookSummary.Statuses.Add(pageSummary);
                }
            }

            return bookSummaries.Values;
        }
    }

    public async Task UpdateIssueContentUrl(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId, string url, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"UPDATE `File` f
                                INNER JOIN IssueContent ic ON ic.FileId = f.Id
                                INNER JOIN Issue i ON i.Id = ic.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            SET FilePath = @ContentUrl
                            WHERE p.LibraryId = @LibraryId 
                                AND i.PeriodicalId = @PeriodicalId
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND ic.id = @ContentId";
            var command = new CommandDefinition(sql, new
            {
                LibraryId = libraryId,
                PeriodicalId = periodicalId,
                VolumeNumber = volumeNumber,
                IssueNumber = issueNumber,
                ContentId = contentId,
                ContentUrl = url
            }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    public async Task DeleteIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, long contentId, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetLibraryConnection())
        {
            var sql = @"DELETE ic
                            FROM IssueContent ic
                                INNER JOIN Issue i ON i.Id = ic.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                INNER JOIN `File` f ON ic.FileId = f.Id
                            WHERE p.LibraryId = @LibraryId 
                                AND p.Id = @PeriodicalId 
                                AND i.VolumeNumber = @VolumeNumber
                                AND i.IssueNumber = @IssueNumber
                                AND ic.Id= @ContentId";
            var command = new CommandDefinition(sql, new { 
                    LibraryId = libraryId, 
                    PeriodicalId = periodicalId, 
                    VolumeNumber = volumeNumber, 
                    IssueNumber = issueNumber,
                    ContentId = contentId
                },
                cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    private async Task<IssueContentModel> GetIssueContentById(IDbConnection connection, long id, CancellationToken cancellationToken)
    {
        var sql = @"SELECT ic.Id, ic.IssueId, ic.Language, 
                                p.Id As PeriodicalId, i.VolumeNumber As VolumeNumber, i.IssueNumber As IssueNumber,
                                f.MimeType, f.Id As FileId, f.FilePath AS ContentUrl
                            FROM IssueContent ic
                                INNER JOIN Issue i ON i.Id = ic.IssueId
                                INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                                INNER JOIN `File` f ON ic.FileId = f.Id
                            WHERE ic.Id = @Id";
        var command = new CommandDefinition(sql, new
        {
            Id = id
        }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<IssueContentModel>(command);
    }

    private static string GetSortByQuery(IssueSortByType sortBy)
    {
        switch (sortBy)
        {
            case IssueSortByType.IssueDate:
                return "IssueDate";

            case IssueSortByType.VolumeNumber:
                return "VolumeNumber";
            case IssueSortByType.VolumeNumberAndIssueNumber:
                return "VolumeNumber, IssueDate";

            default:
                return "IssueDate";
        }
    }
}
