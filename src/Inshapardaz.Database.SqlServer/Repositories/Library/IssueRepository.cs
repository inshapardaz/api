using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class IssueRepository : IIssueRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public IssueRepository(IProvideConnection connectionProvider)
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
                            (SELECT COUNT(*) FROM Article WHERE IssueId = i.id) As ArticleCount,
                            (SELECT COUNT(*) FROM IssuePage WHERE IssueId = i.id) As [PageCount],
                            p.*
                            FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            LEFT OUTER JOIN [File] f ON f.Id = i.ImageId
                            Where p.LibraryId = @LibraryId
                            AND p.Id = @PeriodicalId
                            AND (@Year IS NULL OR YEAR(IssueDate) = @Year)
                            AND (@VolumeNumber IS NULL OR VolumeNumber = @VolumeNumber) " +
                            $" ORDER BY {sortByQuery} {direction} " +
                            @"OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new {
                                                        LibraryId = libraryId,
                                                        PeriodicalId = periodicalId,
                                                        PageSize = pageSize,
                                                        PageNumber = pageNumber,
                                                        Year = filter.Year.HasValue ? filter.Year : null,
                                                        VolumeNumber = filter.VolumeNumber.HasValue ? filter.VolumeNumber : null
                                                    },
                                                        
                                                    cancellationToken: cancellationToken);

                var issues = await connection.QueryAsync<IssueModel, PeriodicalModel, IssueModel>(command, (i, periodical) =>
                {
                    i.Periodical = periodical;
                    i.Frequency = periodical.Frequency;
                    return i;
                });

                var sqlAuthorCount = @"SELECT COUNT(*) FROM Issue as i
                        INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                        WHERE p.LibraryId = @LibraryId 
                        AND p.Id = @PeriodicalId";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, PeriodicalId = periodicalId}, cancellationToken: cancellationToken));

                return new Page<IssueModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = issues
                };
            }
        }

        public async Task<IssueModel> GetIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT i.*, p.*, f.FilePath as ImageUrl,
                            (SELECT COUNT(*) FROM Article WHERE IssueId = i.id) As ArticleCount,
                            (SELECT COUNT(*) FROM IssuePage WHERE IssueId = i.id) As [PageCount]
                            FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            LEFT OUTER JOIN [File] f ON f.Id = i.ImageId
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

                var result = await connection.QueryAsync<IssueModel, PeriodicalModel, string, int, int, IssueModel>(command, (i, p, iUrl, ac, pc) =>
                {
                    i.Periodical = p;
                    i.Frequency = p.Frequency;
                    i.ArticleCount = ac;
                    i.PageCount = pc;
                    i.ImageUrl = iUrl;
                    return i;
                }, splitOn: "Id, ImageUrl, ArticleCount, PageCount") ;

                return result.SingleOrDefault();
            }
        }

        public async Task<IssueModel> AddIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = "INSERT Into Issue (PeriodicalId, Volumenumber, IssueNumber, IsPublic, ImageId, IssueDate) " +
                          "OUTPUT Inserted.Id " +
                          "VALUES (@PeriodicalId, @Volumenumber, @IssueNumber, @IsPublic, @ImageId, @IssueDate)";
                var parameter = new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    Volumenumber = issue.VolumeNumber,
                    IssueNumber = issue.IssueNumber,
                    IssueDate = issue.IssueDate,
                    IsPublic = issue.IsPublic,
                    ImageId = issue.ImageId
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetIssueById(libraryId, periodicalId, id, cancellationToken);
        }

        public async Task UpdateIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Issue
                            SET PeriodicalId = @PeriodicalId, Volumenumber = @Volumenumber, IssueNumber = @IssueNumber, 
                                IssueDate = @IssueDate, IsPublic = @IsPublic, ImageId = @ImageId
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
                    ImageId = issue.ImageId
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"DELETE i FROM Issue i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId 
                            AND p.Id = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, PeriodicalId = periodicalId, VolumeNumber = volumeNumber, IssueNumber = issueNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task AddIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int fileId, string language, string mimeType, CancellationToken cancellationToken)
        {
            var issue = await GetIssue(libraryId, periodicalId, volumeNumber, issueNumber,cancellationToken);
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO IssueContent (IssueId, FileId, Language, MimeType)
                            VALUES (@IssueId, @FileId, @Language, @MimeType)";
                var command = new CommandDefinition(sql, new { FileId = fileId, IssueId = issue.Id, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<IssueContentModel>> GetIssueContents(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT ic.Id, ic.IssueId, i.PeriodicalId, i.VolumeNumber, i.IssueNumber, ic.Language, f.MimeType, f.Id As FileId, f.FilePath AS ContentUrl
                            FROM IssueContent ic
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            INNER JOIN [File] f ON ic.FileId = f.Id
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

        public async Task<IssueContentModel> GetIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT ic.Id, ic.IssueId, ic.Language, 
                            p.Id As PeriodicalId, i.VolumeNumber As VolumeNumber, i.IssueNumber As IssueNumber,
                            f.MimeType, f.Id As FileId, f.FilePath AS ContentUrl
                            FROM IssueContent ic
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            INNER JOIN [File] f ON ic.FileId = f.Id
                            WHERE p.LibraryId = @LibraryId 
                            AND i.VolumeNumber = @VolumeNumber 
                            AND i.IssueNumber = @IssueNumber 
                            AND i.PeriodicalId = @PeriodicalId 
                            AND ic.Language = @Language 
                            AND f.MimeType = @MimeType";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, PeriodicalId = periodicalId, 
                                    VolumeNumber = volumeNumber, IssueNumber = issueNumber, 
                                    Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<IssueContentModel>(command);
            }
        }

        public async Task UpdateIssueContentUrl(int libraryId, int periodicalId, int volumeNumber, int issueNumber, string language, string mimeType, string url, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE f SET FilePath = @ContentUrl
                            FROM  [File] f
                            INNER JOIN IssueContent ic ON ic.FileId = f.Id
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId 
                            AND i.PeriodicalId = @PeriodicalId
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND f.MimeType  = @MimeType 
                            AND ic.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    VolumeNumber = volumeNumber,
                    IssueNumber = issueNumber,
                    Language = language,
                    MimeType = mimeType,
                    ContentUrl = url
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"DELETE ic
                            FROM IssueContent ic
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            INNER JOIN [File] f ON ic.FileId = f.Id
                            WHERE p.LibraryId = @LibraryId 
                            AND p.Id = @PeriodicalId 
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND f.MimeType = @MimeType 
                            AND ic.Language = @Language";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, PeriodicalId = periodicalId, Language = language, MimeType = mimeType, VolumeNumber = volumeNumber, IssueNumber = issueNumber }, 
                                cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public Task UpdateIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int articleId, string language, string mimeType, string actualUrl, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private async Task<IssueModel> GetIssueById(int libraryId, int periodicalId, int issueId,CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT i.*, p.*, f.FilePath as ImageUrl,
                            (SELECT COUNT(*) FROM Article WHERE IssueId = i.id) As ArticleCount,
                            (SELECT COUNT(*) FROM IssuePage WHERE IssueId = i.id) As [PageCount]
                            FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            LEFT OUTER JOIN [File] f ON f.Id = i.ImageId
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

                var result = await connection.QueryAsync<IssueModel, PeriodicalModel, string, int, int, IssueModel>(command, (i, p, iUrl, ac, pc) =>
                {
                    i.Periodical = p;
                    i.ArticleCount = ac;
                    i.PageCount = pc;
                    i.ImageUrl = iUrl;
                    return i;
                }, splitOn: "Id, ImageUrl, ArticleCount, PageCount");

                return result.SingleOrDefault();
            }
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
}
