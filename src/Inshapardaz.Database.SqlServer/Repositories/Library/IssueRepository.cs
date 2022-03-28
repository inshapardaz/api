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
                var sql = @"SELECT i.*
                            FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
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

        public async Task<IssueModel> GetIssueById(int libraryId, int periodicalId, int issueId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT i.*
                            FROM Issue as i
                            INNER JOIN Periodical p ON p.Id = i.PeriodicalId
                            WHERE p.LibraryId = @LibraryId
                            AND p.Id = @PeriodicalId
                            AND i.IssueNumber = @IssueId";
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

        public async Task<IssueModel> AddIssue(int libraryId, int periodicalId, IssueModel issue, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into Issue (PeriodicalId, Volumenumber, IssueNumber, IsPublic, ImageId, IssueDate) Output Inserted.Id Values (@PeriodicalId, @Volumenumber, @IssueNumber, @IsPublic, @ImageId, @IssueDate)";
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
                var sql = @"Update Issue
                            Set PeriodicalId = @PeriodicalId, Volumenumber = @Volumenumber, IssueNumber = @IssueNumber, IssueDate = @IssueDate, IsPublic = @IsPublic, ImageId = @ImageId
                            Where Id = @Id AND LibraryId = @LibraryId";
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

        public async Task DeleteIssue(int libraryId, int issueId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Issue Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = issueId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task AddIssueContent(int libraryId, int periodicalId, int issueId, int fileId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into IssueContent (PeriodicalId, IssueId, FileId, Language, MimeType)
                            Values (@PeriodicalId, @IssueId, @FileId, @Language, @MimeType)";
                var command = new CommandDefinition(sql, new { FileId = fileId, PeriodicalId = periodicalId, IssueId = issueId, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task<IssueContentModel> GetIssueContent(int libraryId, int periodicalId, int issueId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT ic.Id, ic.IssueId, ic.Language, f.MimeType, f.Id As FileId, f.FilePath As ContentUrl
                            FROM IssueContent ic
                            INNER JOIN Issue i ON i.Id = ic.IssueId
                            INNER JOIN [File] f ON ic.FileId = f.Id
                            WHERE b.LibraryId = @LibraryId AND ic.IssueId = @IssueIdAND AND i.PeriodicalId = @PeriodicalId AND ic.Language = @Language AND f.MimeType = @MimeType";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, PeriodicalId = periodicalId, IssueId = issueId, Language = language, MimeType = mimeType }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<IssueContentModel>(command);
            }
        }

        public async Task UpdateIssueContentUrl(int libraryId, int periodicalId, int issueId, string language, string mimeType, string url, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update f SET FilePath = @ContentUrl
                            From  [File] f
                            Inner Join IssueContent ic On ic.FileId = f.Id
                            Inner Join Issue i On i.Id = ic.IssueId
                            Where b.LibraryId = @LibraryId and i.Id = @IssueId And f.MimeType  = @MimeType AND bc.Language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    LibraryId = libraryId,
                    IssueId = issueId,
                    Language = language,
                    MimeType = mimeType,
                    ContentUrl = url
                }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public async Task DeleteIssueContent(int libraryId, int periodicalId, int issueId, string language, string mimeType, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete ic
                            From IssueContent ic
                            Inner Join Issue i On i.Id = ic.IssueId
                            INNER JOIN [File] f ON bc.FileId = f.Id
                            Where b.LibraryId = @LibraryId and i.Id = @IssueId And f.MimeType = @MimeType AND bc.Language = @Language";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Language = language, MimeType = mimeType, IssueId = issueId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        public Task UpdateIssueContent(int libraryId, int periodicalId, int issueId, int articleId, string language, string mimeType, string actualUrl, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
