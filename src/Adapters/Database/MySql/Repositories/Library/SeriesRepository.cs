using Dapper;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public SeriesRepository(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<SeriesModel> AddSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Insert Into Series (`Name`, `Description`, ImageId, LibraryId) 
                            VALUES (@Name, @Description, @ImageId, @LibraryId);
                            SELECT LAST_INSERT_ID();";
                var parameter = new { LibraryId = libraryId, Name = series.Name, Description = series.Description, ImageId = series.ImageId };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetSeriesById(libraryId, id, cancellationToken);
        }

        public async Task UpdateSeries(int libraryId, SeriesModel series, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Series 
                            SET `Name` = @Name, 
                                `Description` = @Description, 
                                ImageId = @ImageId, 
                                LibraryId = @LibraryId 
                            WHERE Id = @Id
                                AND LibraryId = @LibraryId";
                var parameter = new { LibraryId = libraryId, Name = series.Name, Description = series.Description, ImageId = series.ImageId, Id = series.Id };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeleteSeries(int libraryId, int seriesId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql1 = @"UPDATE Book SET 
                                SeriesIndex = NULL 
                            WHERE LibraryId = @LibraryId 
                                AND SeriesId = @SeriesId";
                var command1 = new CommandDefinition(sql1, new { LibraryId = libraryId, SeriesId = seriesId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command1);

                var sql2 = @"DELETE FROM Series WHERE LibraryId = @LibraryId AND Id = @Id";
                var command2 = new CommandDefinition(sql2, new { LibraryId = libraryId, Id = seriesId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command2);
            }
        }

        public async Task<Page<SeriesModel>> GetSeries(int libraryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT  s.Id, s.Name, s.Description, s.ImageId, f.FilePath AS ImageUrl, 
                                (SELECT COUNT(*) FROM Book b WHERE b.SeriesId = s.Id) AS BookCount
                            FROM Series AS s
                                LEFT OUTER JOIN `File` f ON f.Id = s.ImageId
                            WHERE s.LibraryId = @LibraryId
                            ORDER BY s.Name
                            LIMIT @PageSize OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new
                                                    {
                                                        LibraryId = libraryId,
                                                        PageSize = pageSize,
                                                        Offset = pageSize * (pageNumber - 1)
                                                    },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<SeriesModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Series WHERE LibraryId = @LibraryId";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId }, cancellationToken: cancellationToken));

                return new Page<SeriesModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<Page<SeriesModel>> FindSeries(int libraryId, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT s.Id, s.Name, s.Description, s.ImageId, f.FilePath AS ImageUrl, 
                                (SELECT COUNT(*) FROM Book b WHERE b.SeriesId = s.Id) AS BookCount
                            FROM Series AS s
                                LEFT OUTER JOIN `File` f ON f.Id = s.ImageId
                            WHERE s.LibraryId = @LibraryId 
                                AND s.Name LIKE @Query
                            ORDER BY s.Name
                            LIMIT @PageSize OFFSET @Offset";
                var command = new CommandDefinition(sql,
                                                    new
                                                    {
                                                        LibraryId = libraryId,
                                                        Query = $"%{query}%",
                                                        PageSize = pageSize,
                                                        Offset = pageSize * (pageNumber - 1)
                                                    },
                                                    cancellationToken: cancellationToken);

                var series = await connection.QueryAsync<SeriesModel>(command);

                var sqlAuthorCount = "SELECT COUNT(*) FROM Series WHERE LibraryId = @LibraryId And `Name` LIKE @Query";
                var seriesCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlAuthorCount, new { LibraryId = libraryId, Query = $"%{query}%" }, cancellationToken: cancellationToken));

                return new Page<SeriesModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = seriesCount,
                    Data = series
                };
            }
        }

        public async Task<SeriesModel> GetSeriesById(int libraryId, int seriesId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"Select s.Id, s.`Name`, s.Description, s.ImageId,
                            (SELECT Count(*) FROM Book b WHERE b.SeriesId = s.Id) AS BookCount
                            FROM Series AS s
                            WHERE s.LibraryId = @LibraryId AND s.Id = @id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = seriesId }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<SeriesModel>(command);
            }
        }

        public async Task UpdateSeriesImage(int libraryId, int seriesId, long imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Series SET ImageId = @ImageId WHERE Id = @Id AND LibraryId = @LibraryId ";
                var command = new CommandDefinition(sql, new { Id = seriesId, LibraryId = libraryId, ImageId = imageId }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }
    }
}
