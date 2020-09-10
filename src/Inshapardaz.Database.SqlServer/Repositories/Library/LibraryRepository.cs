using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
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
