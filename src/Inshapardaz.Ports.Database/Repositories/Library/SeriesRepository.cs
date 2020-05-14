using Dapper;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Inshapardaz.Ports.Database.Repositories.Library
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public SeriesRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<SeriesModel> AddSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = "Insert Into Library.Series (Name, [Description], ImageId, LibraryId) Output Inserted.Id VALUES (@Name, @Description, @ImageId, @LibraryId)";
                var parameter = new { LibraryId = libraryId, Name = series.Name, Description = series.Description, ImageId = series.ImageId };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetSeriesById(libraryId, id, cancellationToken);
        }

        public async Task UpdateSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Library.Series Set Name = @Name, [Description] = @Description, ImageId = @ImageId, LibraryId = @LibraryId Where Id = @Id And LibraryId = @LibraryId";
                var parameter = new { LibraryId = libraryId, Name = series.Name, Description = series.Description, ImageId = series.ImageId, Id = series.Id };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteSeries(int libraryId, int seriesId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql1 = @"Update Library.Book SET SeriesIndex = NULL Where LibraryId = @LibraryId AND SeriesId = @SeriesId";
                var command1 = new CommandDefinition(sql1, new { LibraryId = libraryId, SeriesId = seriesId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command1);

                var sql2 = @"Delete From Library.Series Where LibraryId = @LibraryId AND Id = @Id";
                var command2 = new CommandDefinition(sql2, new { LibraryId = libraryId, Id = seriesId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command2);
            }
        }

        public async Task<IEnumerable<SeriesModel>> GetSeries(int libraryId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select s.Id, s.Name, s.[Description], s.ImageId,
                            (Select Count(*) From Library.Book b Where b.SeriesId = s.Id) AS BookCount
                            FROM Library.Series AS s
                            Where LibraryId = @LibraryId";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<SeriesModel>(command);
            }
        }

        public async Task<SeriesModel> GetSeriesById(int libraryId, int seriesId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Select s.Id, s.Name, s.[Description], s.ImageId,
                            (Select Count(*) From Library.Book b Where b.SeriesId = s.Id) AS BookCount
                            FROM Library.Series AS s
                            Where s.LibraryId = @LibraryId AND s.Id = @id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = seriesId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<SeriesModel>(command);
            }
        }
    }
}
