using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
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

        public async Task<Page<IssueModel>> GetIssues(int libraryId, int periodicalId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT i.*, f.FilePath as ImageUrl
                            FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            LEFT OUTER JOIN [File] f ON f.Id = i.ImageId
                            Where p.LibraryId = @LibraryId
                            Order By i.IssueDate
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var periodicals = await connection.QueryAsync<IssueModel>(command);

                var sqlAuthorCount = @"SELECT COUNT(*) FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId WHERE p.LibraryId = @LibraryId";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId }, cancellationToken: cancellationToken));

                return new Page<IssueModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = periodicals
                };
            }
        }

        public async Task<IssueModel> GetIssue(int libraryId, int periodicalId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT i.*, f.FilePath as ImageUrl
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

                return await connection.QuerySingleOrDefaultAsync<IssueModel>(command);
            }
        }

        public async Task<IssueModel> AddIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
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
            using (var connection = _connectionProvider.GetConnection())
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

        public async Task DeleteIssue(int libraryId, int volumeNumber, int issueNumber, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"DELETE FROM Issue WHERE LibraryId = @LibraryId AND VolumeNumber = @VolumeNumber AND IssueNumber = @IssueNumber";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, VolumeNumber = volumeNumber, IssueNumber = issueNumber }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task AddIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, int fileId, string language, string mimeType, CancellationToken cancellationToken)
        {
            var issue = GetIssue(libraryId, periodicalId, volumeNumber, issueNumber,cancellationToken);
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"INSERT INTO IssueContent (PeriodicalId, IssueId, FileId, Language, MimeType)
                            VALUES (@PeriodicalId, @IssueId, @FileId, @Language, @MimeType)";
                var command = new CommandDefinition(sql, new { FileId = fileId, PeriodicalId = periodicalId, VolumeNumber = volumeNumber, IssueId = issue.Id, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IssueContentModel> GetIssueContent(int libraryId, int periodicalId, int volumeNumber, int issueNumber, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT ic.Id, ic.IssueId, ic.Language, f.MimeType, f.Id As FileId, f.FilePath AS ContentUrl
                            FROM IssueContent ic
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN [File] f ON ic.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId 
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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"UPDATE f SET FilePath = @ContentUrl
                            FROM  [File] f
                            INNER JOIN IssueContent ic ON ic.FileId = f.Id
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            WHERE b.LibraryId = @LibraryId 
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND f.MimeType  = @MimeType 
                            AND bc.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"DELETE ic
                            FROM IssueContent ic
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId 
                            AND i.VolumeNumber = @VolumeNumber
                            AND i.IssueNumber = @IssueNumber
                            AND f.MimeType = @MimeType 
                            AND bc.Language = @Language";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Language = language, MimeType = mimeType, VolumeNumber = volumeNumber, IssueNumber = issueNumber }, 
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
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT i.*, f.FilePath as ImageUrl
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

                return await connection.QuerySingleOrDefaultAsync<IssueModel>(command);
            }
        }
    }
}
