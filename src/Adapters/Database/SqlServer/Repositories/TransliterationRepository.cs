using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories;

public class TransliterationRepository(SqlServerConnectionProvider connectionProvider) : ITransliterationRepository
{
    public async Task<IEnumerable<TransliterationEntryModel>> GetAllEntries(string language, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = "SELECT * FROM [Language].TransliterationEntries WHERE [Language] = @Language ORDER BY Roman";
        return await connection.QueryAsync<TransliterationEntryModel>(new CommandDefinition(sql, new { Language = language }, cancellationToken: cancellationToken));
    }

    public async Task<Page<TransliterationEntryModel>> GetEntries(string language, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var parameters = new
        {
            Language = language,
            Query = string.IsNullOrEmpty(query) ? null : $"%{query}%",
            PageSize = pageSize,
            Offset = pageSize * (pageNumber - 1)
        };
        var where = "WHERE [Language] = @Language AND (@Query IS NULL OR Roman LIKE @Query OR Native LIKE @Query)";
        var entries = await connection.QueryAsync<TransliterationEntryModel>(new CommandDefinition(
            $"SELECT * FROM [Language].TransliterationEntries {where} ORDER BY Roman OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", parameters, cancellationToken: cancellationToken));
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            $"SELECT COUNT(*) FROM [Language].TransliterationEntries {where}", parameters, cancellationToken: cancellationToken));

        return new Page<TransliterationEntryModel> { PageNumber = pageNumber, PageSize = pageSize, TotalCount = count, Data = entries };
    }

    public async Task<TransliterationEntryModel> GetEntryById(string language, long id, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = "SELECT * FROM [Language].TransliterationEntries WHERE Id = @Id AND [Language] = @Language";
        return await connection.QuerySingleOrDefaultAsync<TransliterationEntryModel>(new CommandDefinition(sql, new { Id = id, Language = language }, cancellationToken: cancellationToken));
    }

    public async Task<TransliterationEntryModel> GetEntryByRoman(string language, string roman, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = "SELECT TOP 1 * FROM [Language].TransliterationEntries WHERE [Language] = @Language AND Roman = @Roman";
        return await connection.QueryFirstOrDefaultAsync<TransliterationEntryModel>(new CommandDefinition(sql, new { Language = language, Roman = roman }, cancellationToken: cancellationToken));
    }

    public async Task<TransliterationEntryModel> AddEntry(TransliterationEntryModel entry, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"INSERT INTO [Language].TransliterationEntries ([Language], Roman, Native, Meaning, Category)
                    OUTPUT Inserted.Id
                    VALUES (@Language, @Roman, @Native, @Meaning, @Category)";
        var id = await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, entry, cancellationToken: cancellationToken));
        return await GetEntryById(entry.Language, id, cancellationToken);
    }

    public async Task<TransliterationEntryModel> UpdateEntry(TransliterationEntryModel entry, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"UPDATE [Language].TransliterationEntries
                    SET Roman = @Roman, Native = @Native, Meaning = @Meaning, Category = @Category
                    WHERE Id = @Id AND [Language] = @Language";
        await connection.ExecuteAsync(new CommandDefinition(sql, entry, cancellationToken: cancellationToken));
        return await GetEntryById(entry.Language, entry.Id, cancellationToken);
    }

    public async Task DeleteEntry(string language, long id, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = "DELETE FROM [Language].TransliterationEntries WHERE Id = @Id AND [Language] = @Language";
        await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, Language = language }, cancellationToken: cancellationToken));
    }
}
