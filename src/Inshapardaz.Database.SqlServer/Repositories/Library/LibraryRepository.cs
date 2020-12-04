﻿using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories.Library
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public LibraryRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<LibraryModel>> GetLibraries(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *
                            FROM Library
                            Order By Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<LibraryModel>> FindLibraries(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT  *
                            FROM Library
                            WHERE Name LIKE @Query
                            Order By Name
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { Query = $"%{query}%", PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<LibraryModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Library WHERE Name LIKE @Query";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { Query = $"%{query}%" }, cancellationToken: cancellationToken));

                return new Page<LibraryModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<LibraryModel> AddLibrary(LibraryModel library, CancellationToken cancellationToken)
        {
            int libraryId;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Library(Name, Language, SupportsPeriodicals) OUTPUT Inserted.Id VALUES(@Name, @Language, @SupportsPeriodicals);";
                var command = new CommandDefinition(sql, new { Name = library.Name, Language = library.Language, SupportsPeriodicals = library.SupportsPeriodicals }, cancellationToken: cancellationToken);
                libraryId = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetLibraryById(libraryId, cancellationToken);
        }

        public async Task<LibraryModel> GetLibraryById(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM Library Where Id = @LibraryId";
                var command = new CommandDefinition(sql,
                                                    new { LibraryId = libraryId },
                                                    cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<LibraryModel>(command);
            }
        }

        public async Task UpdateLibrary(LibraryModel library, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library Set Name = @Name, Language = @Language, SupportsPeriodicals = @SupportsPeriodicals Where Id = @Id";
                var command = new CommandDefinition(sql, new { Id = library.Id, Name = library.Name, Language = library.Language, SupportsPeriodicals = library.SupportsPeriodicals }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteLibrary(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Library Where Id = @Id";
                var command = new CommandDefinition(sql, new { Id = libraryId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
