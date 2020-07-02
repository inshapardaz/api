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
                var sql = @"SELECT p.*, (SELECT Count(*) FROM Issue i WHERE i.PeriodicalId = p.Id) AS IssueCount
                            FROM Periodical AS p
                            INNER JOIN [File] f ON f.Id = p.ImageId
                            Where p.LibraryId = @LibraryId
                            Order By p.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var periodicals = await connection.QueryAsync<IssueModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Periodical WHERE LibraryId = @LibraryId";
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
                var sql = @"SELECT p.*, (SELECT Count(*) FROM Issue i WHERE i.PeriodicalId = p.Id) AS IssueCount
                            FROM Periodical AS p
                            INNER JOIN [File] f ON f.Id = p.ImageId
                            Where p.LibraryId = @LibraryId
                            And PeriodicalId = @PeriodicalId
                            And IssueId = @IssueId";
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
                var sql = "Insert Into Issue (PeriodicalId, Volumenumber, IssueNumber, ImageId, IssueDate) Output Inserted.Id Values (@PeriodicalId, @Volumenumber, @IssueNumber, @ImageId, @IssueDate)";
                var parameter = new
                {
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    Volumenumber = issue.VolumeNumber,
                    IssueNumber = issue.IssueNumber,
                    IssueDate = issue.IssueDate,
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
                            Set PeriodicalId = @PeriodicalId, Volumenumber = @Volumenumber, IssueNumber = @IssueNumber, IssueDate = @IssueDate, ImageId = @ImageId
                            Where Id = @Id AND LibraryId = @LibraryId";
                var parameter = new
                {
                    Id = issue.Id,
                    LibraryId = libraryId,
                    PeriodicalId = periodicalId,
                    Volumenumber = issue.VolumeNumber,
                    IssueNumber = issue.IssueNumber,
                    IssueDate = issue.IssueDate,
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
    }
}
