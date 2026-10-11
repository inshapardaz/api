using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories;

public class CommonWordsRepository(SqlServerConnectionProvider connectionProvider) : ICommonWordsRepository
{
    public async Task<IEnumerable<string>> GetWordsForLanguage(string language, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"SELECT Word FROM [Library].CommonWords WHERE [Language] = @Language ORDER BY Word";
        var command = new CommandDefinition(sql, new { Language = language }, cancellationToken: cancellationToken);
        return await connection.QueryAsync<string>(command);
    }

    public async Task<Page<CommonWordModel>> GetWords(string language, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"SELECT * FROM [Library].CommonWords
                    WHERE [Language] = @Language AND Word LIKE @Word
                    ORDER BY Word
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        var parameters = new { Language = language, Word = $"%{query}%", PageSize = pageSize, Offset = pageSize * (pageNumber - 1) };
        var words = await connection.QueryAsync<CommonWordModel>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var sqlCount = @"SELECT COUNT(*) FROM [Library].CommonWords WHERE [Language] = @Language AND Word LIKE @Word";
        var wordCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sqlCount, parameters, cancellationToken: cancellationToken));

        return new Page<CommonWordModel>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = wordCount,
            Data = words
        };
    }

    public async Task<CommonWordModel> GetWordById(string language, long id, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"SELECT * FROM [Library].CommonWords WHERE Id = @Id AND [Language] = @Language";
        var command = new CommandDefinition(sql, new { Id = id, Language = language }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<CommonWordModel>(command);
    }

    public async Task<CommonWordModel> AddWord(CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"INSERT INTO [Library].CommonWords ([Language], Word) OUTPUT Inserted.Id VALUES (@Language, @Word)";
        var command = new CommandDefinition(sql, new { commonWordModel.Language, commonWordModel.Word }, cancellationToken: cancellationToken);
        commonWordModel.Id = await connection.ExecuteScalarAsync<long>(command);
        return commonWordModel;
    }

    public async Task<CommonWordModel> UpdateWord(CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"UPDATE [Library].CommonWords SET [Language] = @Language, Word = @Word WHERE Id = @Id";
        await connection.ExecuteAsync(new CommandDefinition(sql, commonWordModel, cancellationToken: cancellationToken));
        return await GetWordById(commonWordModel.Language, commonWordModel.Id, cancellationToken);
    }

    public async Task DeleteWord(string language, long id, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"DELETE FROM [Library].CommonWords WHERE Id = @Id AND [Language] = @Language";
        await connection.ExecuteAsync(new CommandDefinition(sql, new { Language = language, Id = id }, cancellationToken: cancellationToken));
    }

    public async Task AddWords(string language, IEnumerable<string> words, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        if (connection.State != System.Data.ConnectionState.Open) connection.Open();
        using var transaction = connection.BeginTransaction();
        // SQL Server caps a statement at 2100 parameters, so insert in chunks of multi-row VALUES.
        foreach (var chunk in words.Chunk(500))
        {
            var parameters = new DynamicParameters();
            parameters.Add("Language", language);
            var values = chunk.Select((word, i) =>
            {
                parameters.Add($"Word{i}", word);
                return $"(@Language, @Word{i})";
            });
            var sql = $"INSERT INTO [Library].CommonWords ([Language], Word) VALUES {string.Join(", ", values)}";
            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
        }
        transaction.Commit();
    }
}
