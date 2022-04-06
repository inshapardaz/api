using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories
{
    public class CorrectionRepository : ICorrectionRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public CorrectionRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<Dictionary<string, string>> GetAllCorrections(string language, string profile, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT IncorrectText, CorrectText FROM corrections WHERE language = @Language AND [Profile] = @Profile";
                var command = new CommandDefinition(sql, new
                {
                    Language = language,
                    Profile = profile
                }, cancellationToken: cancellationToken);

                var items = await connection.QueryAsync<(string key, string val)>(command);
                return items.ToDictionary(t => t.key, t => t.val);
            }
        }

        public async Task<Page<CorrectionModel>> GetCorrectionList(string language, string query, string profile, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT *
                            FROM Corrections
                            WHERE language = @Language 
                            AND (IncorrectText LIKE @Query OR CorrectText LIKE @Query)
                            AND Profile = @Profile
                            ORDER BY [Language], Profile, IncorrectText
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";
                var command = new CommandDefinition(sql,
                                                    new { Language = language, Query = $"%{query}%", Profile = profile, PageSize = pageSize, PageNumber = pageNumber },
                                                    cancellationToken: cancellationToken);

                var authors = await connection.QueryAsync<CorrectionModel>(command);

                var sqlCorrectionCount = "SELECT COUNT(*) FROM Corrections WHERE Language = @Language AND IncorrectText LIKE @Query AND Profile = @Profile";
                var authorCount = await connection.QuerySingleAsync<int>(new CommandDefinition(sqlCorrectionCount, new { Language = language, Query = $"%{query}%", Profile = profile }, cancellationToken: cancellationToken));

                return new Page<CorrectionModel>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = authorCount,
                    Data = authors
                };
            }
        }

        public async Task<CorrectionModel> GetCorrection(string language, string profile, string incorrectText, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM corrections 
                            WHERE language = @Language 
                            AND [Profile] = @Profile
                            AND IncorrectText = @IncorrectText";
                var command = new CommandDefinition(sql, new
                {
                    Language = language,
                    Profile = profile,
                    IncorrectText = incorrectText,
                }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<CorrectionModel>(command);
            }
        }

        public async Task<CorrectionModel> AddCorrection(CorrectionModel correction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Insert Into Corrections(Language, Profile, IncorrectText, CorrectText) 
                            VALUES(@Language, @Profile, @IncorrectText, @CorrectText);";
                var command = new CommandDefinition(sql, new { Language = correction.Language, Profile = correction.Profile, IncorrectText = correction.IncorrectText, CorrectText = correction.CorrectText }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }

            return await GetCorrection(correction.Language, correction.Profile, correction.IncorrectText, cancellationToken);

        }

        public async Task<CorrectionModel> UpdateCorrection(CorrectionModel correction, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Update Corrections Set CorrectText = @correction Where Language = @Language AND Profile = @Profile AND IncorrectText = @IncorrectText";
                var command = new CommandDefinition(sql, new { Language = correction.Language, Profile = correction.Profile, IncorrectText = correction.IncorrectText, Correction = correction.CorrectText }, cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);

                return await GetCorrection(correction.Language, correction.Profile, correction.IncorrectText, cancellationToken);
            }
        }

        public async Task DeleteCorrection(string language, string profile, string incorrectText, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"Delete From Corrections Where Language = @Language AND Profile = @Profile AND IncorrectText = @IncorrectText";
                var command = new CommandDefinition(sql, new { Language = language, Profile = profile, IncorrectText = incorrectText }, cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }
        }
    }
}
