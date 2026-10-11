using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories;

public class SpellingReplacementRepository(SqlServerConnectionProvider connectionProvider) : ISpellingReplacementRepository
{
    public async Task<IEnumerable<SpellingReplacementModel>> GetReplacements(string language, IEnumerable<string> normalisedSourceWords, CancellationToken cancellationToken)
    {
        var sourceWords = normalisedSourceWords.Distinct().ToArray();
        if (sourceWords.Length == 0) return [];

        using var connection = connectionProvider.GetConnection();
        var sql = @"SELECT [Language], SourceWord, Replacement, [Count]
                    FROM [Language].SpellingReplacements
                    WHERE [Language] = @Language AND SourceWord IN @SourceWords
                    ORDER BY [Count] DESC, LastUsedAt DESC";
        var command = new CommandDefinition(sql, new { Language = language, SourceWords = sourceWords }, cancellationToken: cancellationToken);
        return await connection.QueryAsync<SpellingReplacementModel>(command);
    }

    public async Task RecordReplacement(string language, string normalisedSourceWord, string replacement, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"UPDATE [Language].SpellingReplacements
                    SET [Count] = [Count] + 1, LastUsedAt = GETUTCDATE()
                    WHERE [Language] = @Language AND SourceWord = @SourceWord AND Replacement = @Replacement;
                    IF @@ROWCOUNT = 0
                        INSERT INTO [Language].SpellingReplacements ([Language], SourceWord, Replacement, [Count], LastUsedAt)
                        VALUES (@Language, @SourceWord, @Replacement, 1, GETUTCDATE());";
        var command = new CommandDefinition(sql, new { Language = language, SourceWord = normalisedSourceWord, Replacement = replacement }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
