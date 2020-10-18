using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class PeriodicalRepository : IPeriodicalRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public PeriodicalRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<PeriodicalModel>> GetPeriodicals(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.*, f.FilePath as ImageUrl, (SELECT Count(*) FROM Issue i WHERE i.PeriodicalId = p.Id) AS IssueCount
                            FROM Periodical AS p
                            INNER JOIN [File] f ON f.Id = p.ImageId
                            Where p.LibraryId = @LibraryId
                            Order By p.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var periodicals = await connection.QueryAsync<PeriodicalModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Periodical WHERE LibraryId = @LibraryId";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId }, cancellationToken: cancellationToken));

                return new Page<PeriodicalModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = periodicals
                };
            }
        }

        public async Task<Page<PeriodicalModel>> SearchPeriodicals(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.*, f.FilePath as ImageUrl, (SELECT Count(*) FROM Issue i WHERE i.PeriodicalId = p.Id) AS IssueCount
                            FROM Periodical AS p
                            INNER JOIN [File] f ON f.Id = p.ImageId
                            Where p.LibraryId = @LibraryId
                            And a.Title LIKE @Query
                            Order By p.Title
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId, PageSize = pageSize, PageNumber = pageNumber, Query = query },
                                                    cancellationToken: cancellationToken);

                var periodicals = await connection.QueryAsync<PeriodicalModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Periodical WHERE LibraryId = @LibraryId And a.Title LIKE @Query";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, Query = query }, cancellationToken: cancellationToken));

                return new Page<PeriodicalModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = periodicals
                };
            }
        }

        public async Task<PeriodicalModel> GetPeriodicalById(int libraryId, int periodicalId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT p.*, f.FilePath as ImageUrl, (SELECT Count(*) FROM Issue i WHERE i.PeriodicalId = p.Id) AS IssueCount
                            FROM Periodical AS p
                            LEFT OUTER JOIN [File] f ON f.Id = p.ImageId
                            Where p.LibraryId = @LibraryId
                            And p.Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = periodicalId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<PeriodicalModel>(command);
            }
        }

        public async Task<PeriodicalModel> AddPeriodical(int libraryId, PeriodicalModel periodical, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into Periodical (Title, [Description], CategoryId, ImageId, LibraryId) Output Inserted.Id Values (@Title, @Description, @CategoryId, @ImageId, @LibraryId)";
                var parameter = new
                {
                    LibraryId = libraryId,
                    Title = periodical.Title,
                    Description = periodical.Description,
                    CategoryId = periodical.CategoryId,
                    ImageId = periodical.ImageId,
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetPeriodicalById(libraryId, id, cancellationToken);
        }

        public async Task UpdatePeriodical(int libraryId, PeriodicalModel periodical, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Periodical
                            Set Title = @Title, Description = @Description, CategoryId = @CategoryId, ImageId = @ImageId
                            Where Id = @Id AND LibraryId = @LibraryId";
                var parameter = new
                {
                    Id = periodical.Id,
                    LibraryId = libraryId,
                    Name = periodical.Title,
                    Description = periodical.Description,
                    CategoryId = periodical.CategoryId,
                    ImageId = periodical.ImageId,
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeletePeriodical(int libraryId, int periodicalId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Periodical Where LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = periodicalId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
