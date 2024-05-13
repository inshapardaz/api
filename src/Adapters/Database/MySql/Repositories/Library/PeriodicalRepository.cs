using Dapper;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories.Library
{
    public class PeriodicalRepository : IPeriodicalRepository
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public PeriodicalRepository(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Page<PeriodicalModel>> GetPeriodicals(int libraryId, string query, int pageNumber, int pageSize, PeriodicalFilter filter, PeriodicalSortByType sortBy, SortDirection direction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sortByQuery = $"p.{GetSortByQuery(sortBy)}";
                var sortDirection = direction == SortDirection.Descending ? "DESC" : "ASC";

                var param = new
                {
                    Query = string.IsNullOrWhiteSpace(query) ? null : $"%{query}%",
                    LibraryId = libraryId,
                    PageSize = pageSize,
                    Offset = pageSize * (pageNumber - 1),
                    CategoryFilter = filter.CategoryId,
                    Frequency = filter.Frequency
                };
                var sql = @"Select p.Id, p.Title
                            FROM Periodical p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN PeriodicalCategory pc ON p.Id = pc.PeriodicalId
                                LEFT OUTER JOIN Category c ON c.Id = pc.PeriodicalId
                            WHERE p.LibraryId = @LibraryId 
                                AND (@CategoryFilter IS NULL OR pc.CategoryId = @CategoryFilter)
                                AND (@Query IS NULL OR p.Title LIKE @Query)
                                AND (p.Frequency = @Frequency OR @Frequency IS NULL)
                            GROUP BY p.Id, p.Title " +
                            $" ORDER BY {sortByQuery} {sortDirection} " +
                            @"LIMIT @PageSize OFFSET @Offset";
                var command = new CommandDefinition(sql, param, cancellationToken: cancellationToken);

                var periodicalIds = await connection.QueryAsync(command);

                var sqlCount = @"SELECT Count(*) FROM (Select p.Id
                            FROM Periodical p
                                LEFT OUTER JOIN PeriodicalCategory pc ON p.Id = pc.PeriodicalId
                                LEFT OUTER JOIN Category c ON c.Id = pc.PeriodicalId
                            WHERE p.LibraryId = @LibraryId
                                AND (pc.CategoryId = @CategoryFilter OR @CategoryFilter IS NULL)
                                AND (@Query IS NULL OR p.Title LIKE @Query)
                                AND (p.Frequency = @Frequency OR @Frequency IS NULL)
                            GROUP BY p.Id) AS pcnt";
                var periodicalCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCount, param, cancellationToken: cancellationToken));

                var books = await GetPeriodicals(connection, libraryId, periodicalIds.Select(p => (int)p.Id).ToList(), cancellationToken);

                return new Page<PeriodicalModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = periodicalCount,
                    Data = books
                };
            }
        }

        public async Task<PeriodicalModel> GetPeriodicalById(int libraryId, int periodicalId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"SELECT p.*, 
                                f.FilePath as ImageUrl, 
                                (SELECT Count(*) FROM Issue i WHERE i.PeriodicalId = p.Id) AS IssueCount,
                                c.*
                            FROM Periodical AS p
                                LEFT OUTER JOIN `File` f ON f.Id = p.ImageId
                                LEFT OUTER JOIN PeriodicalCategory pc ON p.Id = pc.PeriodicalId
                                LEFT OUTER JOIN Category c ON c.Id = pc.CategoryId
                            WHERE p.LibraryId = @LibraryId
                                AND p.Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = periodicalId }, cancellationToken: cancellationToken);

                PeriodicalModel periodical = null;
                await connection.QueryAsync<PeriodicalModel, CategoryModel, PeriodicalModel>(command, (p, c) =>
                {
                    if (periodical == null)
                    {
                        periodical = p;
                    }

                    if (c != null && !periodical.Categories.Any(x => x.Id == c.Id))
                    {
                        periodical.Categories.Add(c);
                    }

                    return periodical;
                });

                return periodical;
            }
        }

        public async Task<PeriodicalModel> AddPeriodical(int libraryId, PeriodicalModel periodical, CancellationToken cancellationToken)
        {
            int id;
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"INSERT INTO Periodical (`Title`, `Description`, `Language`, ImageId, LibraryId, Frequency) 
                            VALUES (@Title, @Description, @Language, @ImageId, @LibraryId, @Frequency);
                            SELECT LAST_INSERT_ID();";
                var parameter = new
                {
                    LibraryId = libraryId,
                    Title = periodical.Title,
                    Description = periodical.Description,
                    Language = periodical.Language,
                    ImageId = periodical.ImageId,
                    Frequency = periodical.Frequency
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                id = await connection.ExecuteScalarAsync<int>(command);

                var sqlCategory = @"INSERT INTO PeriodicalCategory (PeriodicalId, CategoryId) VALUES (@PeriodicalId, @CategoryId);";

                if (periodical.Categories != null && periodical.Categories.Any())
                {
                    var periodicalCategories = periodical.Categories.Select(c => new { PeriodicalId = id, CategoryId = c.Id });
                    var commandCategory = new CommandDefinition(sqlCategory, periodicalCategories, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }
            }

            return await GetPeriodicalById(libraryId, id, cancellationToken);
        }

        public async Task<PeriodicalModel> UpdatePeriodical(int libraryId, PeriodicalModel periodical, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Periodical
                            SET Title = @Title, 
                                Description = @Description, 
                                `Language` = @Language,
                                Frequency = @Frequency
                            WHERE Id = @Id 
                                AND LibraryId = @LibraryId";
                var parameter = new
                {
                    Id = periodical.Id,
                    LibraryId = libraryId,
                    Title = periodical.Title,
                    Description = periodical.Description,
                    Language = periodical.Language,
                    Frequency = periodical.Frequency
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);

                await connection.ExecuteAsync(new CommandDefinition(
                    "DELETE FROM PeriodicalCategory WHERE PeriodicalId = @PeriodicalId",
                    new { PeriodicalId = periodical.Id },
                    cancellationToken: cancellationToken));

                var sqlCategory = @"INSERT INTO PeriodicalCategory (PeriodicalId, CategoryId) VALUES (@PeriodicalId, @CategoryId);";

                if (periodical.Categories != null && periodical.Categories.Any())
                {
                    var periodicalCategories = periodical.Categories.Select(c => new { PeriodicalId = periodical.Id, CategoryId = c.Id });
                    var commandCategory = new CommandDefinition(sqlCategory, periodicalCategories, cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(commandCategory);
                }
            }

            return await GetPeriodicalById(libraryId, periodical.Id, cancellationToken);
        }

        public async Task UpdatePeriodicalImage(int libraryId, int periodicalId, long imageId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"UPDATE Periodical
                            SET Imageid = @ImageId
                            WHERE Id = @Id 
                                AND LibraryId = @LibraryId";
                var parameter = new
                {
                    Id = periodicalId,
                    LibraryId = libraryId,
                    Imageid = imageId
                };
                var command = new CommandDefinition(sql, parameter, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task DeletePeriodical(int libraryId, int periodicalId, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetLibraryConnection())
            {
                var sql = @"DELETE FROM Periodical WHERE LibraryId = @LibraryId AND Id = @Id";
                var command = new CommandDefinition(sql, new { LibraryId = libraryId, Id = periodicalId }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }

        private async Task<IEnumerable<PeriodicalModel>> GetPeriodicals(IDbConnection connection, int libraryId, List<int> periodicalIds, CancellationToken cancellationToken)
        {
            var periodicals = new Dictionary<int, PeriodicalModel>();
            var sql = @"Select p.*, fl.FilePath AS ImageUrl,
                                (SELECT COUNT(*) FROM Issue WHERE Issue.PeriodicalId = p.id) As IssueCount,
                                c.*
                            FROM Periodical p
                                LEFT OUTER JOIN PeriodicalCategory pc ON p.Id = pc.PeriodicalId
                                LEFT OUTER JOIN Category c ON c.Id = pc.CategoryId
                                LEFT OUTER JOIN `File` fl ON fl.Id = p.ImageId
                            WHERE p.LibraryId = @LibraryId
                                AND p.Id IN @PerioidcalList";
            var command = new CommandDefinition(sql, new { LibraryId = libraryId, PerioidcalList = periodicalIds }, cancellationToken: cancellationToken);

            await connection.QueryAsync<PeriodicalModel, CategoryModel, PeriodicalModel>(command, (p, c) =>
            {
                if (!periodicals.TryGetValue(p.Id, out PeriodicalModel periodical))
                    periodicals.Add(p.Id, periodical = p);

                if (c != null && !periodical.Categories.Any(x => x.Id == c.Id))
                {
                    periodical.Categories.Add(c);
                }

                return periodical;
            });

            return periodicals.Values.OrderBy(p => periodicalIds.IndexOf(p.Id)).ToList();
        }

        private static string GetSortByQuery(PeriodicalSortByType sortBy)
        {
            return "Title";

            //switch (sortBy)
            //{
            //    case PeriodicalSortByType.DateCreated:
            //        return "DateAdded";

            //    default:
            //        return "Title";
            //}
        }
    }
}
