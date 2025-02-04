﻿using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Adapters.Database.MySql.Repositories;

public class CorrectionRepository : ICorrectionRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public CorrectionRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<CorrectionModel>> GetAllCorrections(string language, string profile, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"SELECT * FROM Corrections 
                    WHERE `Language` = @Language 
                        AND `Profile` = @Profile";
            var command = new CommandDefinition(sql, new
            {
                Language = language,
                Profile = profile
            }, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CorrectionModel>(command);
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
                                AND `Profile` = @Profile
                            ORDER BY `Language`, `Profile`, IncorrectText
                            LIMIT @PageSize OFFSET @Offset";
            var command = new CommandDefinition(sql, new { 
                                                    Language = language, 
                                                    Query = $"%{query}%", 
                                                    Profile = profile, 
                                                    PageSize = pageSize, 
                                                    Offset = pageSize * (pageNumber - 1) },
                                                cancellationToken: cancellationToken);

            var authors = await connection.QueryAsync<CorrectionModel>(command);

            var sqlCorrectionCount = @"SELECT COUNT(*) 
                    FROM Corrections 
                    WHERE `Language` = @Language 
                        AND IncorrectText LIKE @Query 
                        AND `Profile` = @Profile";
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

    public async Task<CorrectionModel> GetCorrection(string language, string profile, long id, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"SELECT * FROM Corrections 
                            WHERE Id = @Id
                                AND `Language` = @Language 
                                AND `Profile` = @Profile";
            var command = new CommandDefinition(sql, new
            {
                Id = id,
                Language = language,
                Profile = profile
            }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<CorrectionModel>(command);
        }
    }

    private async Task<CorrectionModel> GetCorrectionById(long id, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"SELECT * FROM Corrections 
                            WHERE Id = @Id";
            var command = new CommandDefinition(sql, new
            {
                Id = id,
            }, cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<CorrectionModel>(command);
        }
    }

    public async Task<CorrectionModel> AddCorrection(CorrectionModel correction, CancellationToken cancellationToken)
    {
        var id = 0;
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"Insert Into Corrections(`Language`, `Profile`, IncorrectText, CorrectText, CompleteWord)  
                            VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord);
                            SELECT LAST_INSERT_ID();";
            var command = new CommandDefinition(sql, correction, cancellationToken: cancellationToken);
            id = await connection.ExecuteScalarAsync<int>(command);
        }

        return await GetCorrectionById(id, cancellationToken);

    }

    public async Task<CorrectionModel> UpdateCorrection(CorrectionModel correction, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"UPDATE Corrections 
                            SET IncorrectText  = @IncorrectText, 
                                CorrectText = @CorrectionText, 
                                CompleteWord = @CompleteWord 
                            WHERE Id = @Id";
            var command = new CommandDefinition(sql, new
            {
                Id = correction.Id,
                IncorrectText = correction.IncorrectText,
                CorrectionText = correction.CorrectText,
                CompleteWord = correction.CompleteWord
            }, cancellationToken: cancellationToken);
            await connection.ExecuteScalarAsync<int>(command);

            return await GetCorrectionById(correction.Id, cancellationToken);
        }
    }

    public async Task DeleteCorrection(long id, CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"DELETE FROM Corrections WHERE Id = @Id";
            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }
    }

    #region for migration
    public async Task<IEnumerable<CorrectionModel>> GetAllCorrections(CancellationToken cancellationToken)
    {
        using (var connection = _connectionProvider.GetConnection())
        {
            var sql = @"SELECT * FROM Corrections";
            var command = new CommandDefinition(sql, cancellationToken: cancellationToken);

            return await connection.QueryAsync<CorrectionModel>(command);
        }
    }
    #endregion
}
