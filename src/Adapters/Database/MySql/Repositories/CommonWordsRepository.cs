using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Adapters.Database.MySql.Repositories;

public class CommonWordsRepository : ICommonWordsRepository
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public CommonWordsRepository(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }
    
    public async Task<IEnumerable<string>> GetWordsForLanguage(string language, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection();
        var sql = @"SELECT `Word` FROM CommonWords WHERE `Language` = @Language ORDER By `Word`";
        var command = new CommandDefinition(sql, new
        {
            Language = language,
        }, cancellationToken: cancellationToken);

        return await connection.QueryAsync<string>(command);
    }

    public async Task<Page<CommonWordModel>> GetWords(string language, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection();
        var sql = @"SELECT * FROM CommonWords 
                    WHERE `Language` = @Language
                    AND `Word` LIKE @Word
                    ORDER BY `Word`
                    LIMIT @PageSize OFFSET @Offset";
        var command = new CommandDefinition(sql, new
        {
            Language = language,
            Word = $"%{query}%",
            PageSize = pageSize,
            Offset = pageSize * (pageNumber - 1),
            
        }, cancellationToken: cancellationToken);

        var words = await connection.QueryAsync<CommonWordModel>(command);
        
        var sqlCount = @"SELECT COUNT(*) FROM CommonWords 
                    WHERE `Language` = @Language
                    AND `Word` LIKE @Word";
        var commandCount = new CommandDefinition(sqlCount, new
        {
            Language = language,
            Word = $"%{query}%",
            PageSize = pageSize,
            Offset = pageSize * (pageNumber - 1),
            
        }, cancellationToken: cancellationToken);
        var wordCount = await connection.ExecuteScalarAsync<int>(commandCount);
        return new Page<CommonWordModel>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = wordCount,
            Data = 
                words
        };
    }

    public async Task<CommonWordModel> GetWordById(string language, long id, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection();
        var sql = @"SELECT * FROM CommonWords WHERE `Id` = @Id AND Language = @Language";
        var command = new CommandDefinition(sql, new
        {
            Id = id,
            Language = language,
        }, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<CommonWordModel>(command);
    }

    public async Task<CommonWordModel> AddWord(CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection();
        var sql = @"Insert Into CommonWords(`Language`, `Word`) VALUES (@Language, @Word);
                    SELECT LAST_INSERT_ID();";
        var command = new CommandDefinition(sql, new
        {
            Language = commonWordModel.Language,
            Word = commonWordModel.Word,
        }, cancellationToken: cancellationToken);
        commonWordModel.Id = await connection.ExecuteScalarAsync<long>(command);
        
        return commonWordModel;
    }

    public async Task<CommonWordModel> UpdateWord(CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection();
        var sql = @"UPDATE CommonWords 
                            SET Language  = @Language, 
                                Word = @Word
                            WHERE Id = @Id";
        var command = new CommandDefinition(sql, commonWordModel, cancellationToken: cancellationToken);
        await connection.ExecuteScalarAsync<int>(command);

        return await GetWordById(commonWordModel.Language, commonWordModel.Id, cancellationToken);
    }

    public async Task DeleteWord(string language, long id, CancellationToken cancellationToken)
    {
        using var connection = _connectionProvider.GetConnection();
        var sql = @"DELETE FROM CommonWords WHERE Id = @Id AND `Language` = @Language";
        var command = new CommandDefinition(sql, new { Language = language, Id = id }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
