using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Adapters.Database.MySql.Repositories;

public class DictionaryRepository(MySqlConnectionProvider connectionProvider) : IDictionaryRepository
{
    private const string ArticleColumns = "a.`Id`, a.`DictionaryId`, a.`Headword`, a.`HeadwordNormalised`, a.`Pronunciation`, a.`Senses`, a.`Links`, a.`RawDefinition`, a.`Alternates`";

    public async Task<Page<DictionaryModel>> GetDictionaries(string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var parameters = new { Query = $"%{query}%", PageSize = pageSize, Offset = pageSize * (pageNumber - 1) };
        var sql = @"SELECT * FROM Dictionaries
                    WHERE @Query = '%%' OR `Name` LIKE @Query
                    ORDER BY `Name`
                    LIMIT @PageSize OFFSET @Offset";
        var dictionaries = await connection.QueryAsync<DictionaryModel>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var sqlCount = "SELECT COUNT(*) FROM Dictionaries WHERE @Query = '%%' OR `Name` LIKE @Query";
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sqlCount, parameters, cancellationToken: cancellationToken));

        return new Page<DictionaryModel> { PageNumber = pageNumber, PageSize = pageSize, TotalCount = count, Data = dictionaries };
    }

    public async Task<DictionaryModel> GetDictionaryById(long dictionaryId, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = "SELECT * FROM Dictionaries WHERE `Id` = @Id";
        return await connection.QuerySingleOrDefaultAsync<DictionaryModel>(new CommandDefinition(sql, new { Id = dictionaryId }, cancellationToken: cancellationToken));
    }

    public async Task<DictionaryModel> AddDictionary(DictionaryModel dictionary, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"INSERT INTO Dictionaries (`Name`, `Description`, `Format`, `SourceLanguage`, `TargetLanguage`, `EntryCount`, `OwnerId`, `OriginalFileName`, `DateCreated`)
                    VALUES (@Name, @Description, @Format, @SourceLanguage, @TargetLanguage, @EntryCount, @OwnerId, @OriginalFileName, UTC_TIMESTAMP());
                    SELECT LAST_INSERT_ID();";
        var id = await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, Parameters(dictionary), cancellationToken: cancellationToken));
        return await GetDictionaryById(id, cancellationToken);
    }

    public async Task<DictionaryModel> UpdateDictionary(DictionaryModel dictionary, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"UPDATE Dictionaries
                    SET `Name` = @Name, `Description` = @Description, `SourceLanguage` = @SourceLanguage, `TargetLanguage` = @TargetLanguage
                    WHERE `Id` = @Id";
        await connection.ExecuteAsync(new CommandDefinition(sql, Parameters(dictionary), cancellationToken: cancellationToken));
        return await GetDictionaryById(dictionary.Id, cancellationToken);
    }

    public async Task DeleteDictionary(long dictionaryId, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Dictionaries WHERE `Id` = @Id", new { Id = dictionaryId }, cancellationToken: cancellationToken));
    }

    public async Task UpdateEntryCount(long dictionaryId, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"UPDATE Dictionaries
                    SET `EntryCount` = (SELECT COUNT(*) FROM DictionaryArticles WHERE `DictionaryId` = @Id)
                    WHERE `Id` = @Id";
        await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = dictionaryId }, cancellationToken: cancellationToken));
    }

    public async Task AddArticles(long dictionaryId, IEnumerable<DictionaryArticleModel> articles, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        using var transaction = connection.BeginTransaction();
        // Multi-row VALUES in chunks: imports can hold hundreds of thousands of articles.
        foreach (var chunk in articles.Select(a => DictionaryArticleRow.From(a, dictionaryId)).Chunk(500))
        {
            var parameters = new DynamicParameters();
            var values = chunk.Select((row, i) =>
            {
                parameters.Add($"DictionaryId{i}", row.DictionaryId);
                parameters.Add($"Headword{i}", row.Headword);
                parameters.Add($"HeadwordNormalised{i}", row.HeadwordNormalised);
                parameters.Add($"Pronunciation{i}", row.Pronunciation);
                parameters.Add($"Senses{i}", row.Senses);
                parameters.Add($"Links{i}", row.Links);
                parameters.Add($"RawDefinition{i}", row.RawDefinition);
                parameters.Add($"Alternates{i}", row.Alternates);
                return $"(@DictionaryId{i}, @Headword{i}, @HeadwordNormalised{i}, @Pronunciation{i}, @Senses{i}, @Links{i}, @RawDefinition{i}, @Alternates{i})";
            });
            var sql = "INSERT INTO DictionaryArticles (`DictionaryId`, `Headword`, `HeadwordNormalised`, `Pronunciation`, `Senses`, `Links`, `RawDefinition`, `Alternates`) VALUES "
                + string.Join(", ", values);
            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
        }
        transaction.Commit();
    }

    public async Task<Page<DictionaryArticleModel>> GetArticles(long dictionaryId, string normalisedQuery, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var parameters = new
        {
            DictionaryId = dictionaryId,
            Query = string.IsNullOrEmpty(normalisedQuery) ? null : $"%{normalisedQuery}%",
            PageSize = pageSize,
            Offset = pageSize * (pageNumber - 1)
        };
        var sql = $@"SELECT {ArticleColumns} FROM DictionaryArticles a
                    WHERE a.`DictionaryId` = @DictionaryId AND (@Query IS NULL OR a.`HeadwordNormalised` LIKE @Query)
                    ORDER BY a.`HeadwordNormalised`, a.`Id`
                    LIMIT @PageSize OFFSET @Offset";
        var rows = await connection.QueryAsync<DictionaryArticleRow>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var sqlCount = @"SELECT COUNT(*) FROM DictionaryArticles a
                         WHERE a.`DictionaryId` = @DictionaryId AND (@Query IS NULL OR a.`HeadwordNormalised` LIKE @Query)";
        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sqlCount, parameters, cancellationToken: cancellationToken));

        return new Page<DictionaryArticleModel> { PageNumber = pageNumber, PageSize = pageSize, TotalCount = count, Data = rows.Select(r => r.ToModel()).ToList() };
    }

    public async Task<IReadOnlyList<DictionaryArticleModel>> GetAllArticles(long dictionaryId, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = $"SELECT {ArticleColumns} FROM DictionaryArticles a WHERE a.`DictionaryId` = @DictionaryId ORDER BY a.`HeadwordNormalised`, a.`Id`";
        var rows = await connection.QueryAsync<DictionaryArticleRow>(new CommandDefinition(sql, new { DictionaryId = dictionaryId }, cancellationToken: cancellationToken));
        return rows.Select(r => r.ToModel()).ToList();
    }

    public async Task<DictionaryArticleModel> GetArticleById(long dictionaryId, long articleId, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = $"SELECT {ArticleColumns} FROM DictionaryArticles a WHERE a.`DictionaryId` = @DictionaryId AND a.`Id` = @Id";
        var row = await connection.QuerySingleOrDefaultAsync<DictionaryArticleRow>(new CommandDefinition(sql, new { DictionaryId = dictionaryId, Id = articleId }, cancellationToken: cancellationToken));
        return row?.ToModel();
    }

    public async Task<DictionaryArticleModel> AddArticle(DictionaryArticleModel article, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"INSERT INTO DictionaryArticles (`DictionaryId`, `Headword`, `HeadwordNormalised`, `Pronunciation`, `Senses`, `Links`, `RawDefinition`, `Alternates`)
                    VALUES (@DictionaryId, @Headword, @HeadwordNormalised, @Pronunciation, @Senses, @Links, @RawDefinition, @Alternates);
                    SELECT LAST_INSERT_ID();";
        var id = await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, DictionaryArticleRow.From(article, article.DictionaryId), cancellationToken: cancellationToken));
        return await GetArticleById(article.DictionaryId, id, cancellationToken);
    }

    public async Task<DictionaryArticleModel> UpdateArticle(DictionaryArticleModel article, CancellationToken cancellationToken)
    {
        using var connection = connectionProvider.GetConnection();
        var sql = @"UPDATE DictionaryArticles
                    SET `Headword` = @Headword, `HeadwordNormalised` = @HeadwordNormalised, `Pronunciation` = @Pronunciation,
                        `Senses` = @Senses, `Links` = @Links, `RawDefinition` = @RawDefinition, `Alternates` = @Alternates
                    WHERE `Id` = @Id AND `DictionaryId` = @DictionaryId";
        await connection.ExecuteAsync(new CommandDefinition(sql, DictionaryArticleRow.From(article, article.DictionaryId), cancellationToken: cancellationToken));
        return await GetArticleById(article.DictionaryId, article.Id, cancellationToken);
    }

    public async Task DeleteArticles(long dictionaryId, IEnumerable<long> articleIds, CancellationToken cancellationToken)
    {
        var ids = articleIds.ToArray();
        if (ids.Length == 0) return;

        using var connection = connectionProvider.GetConnection();
        var sql = "DELETE FROM DictionaryArticles WHERE `DictionaryId` = @DictionaryId AND `Id` IN @Ids";
        await connection.ExecuteAsync(new CommandDefinition(sql, new { DictionaryId = dictionaryId, Ids = ids }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<DictionaryArticleModel>> LookupArticles(string normalisedHeadword, IEnumerable<long> dictionaryIds, CancellationToken cancellationToken)
    {
        var ids = dictionaryIds?.ToArray();
        using var connection = connectionProvider.GetConnection();
        var sql = $@"SELECT {ArticleColumns}, d.`Name` AS DictionaryName
                    FROM DictionaryArticles a
                    INNER JOIN Dictionaries d ON d.`Id` = a.`DictionaryId`
                    WHERE a.`HeadwordNormalised` = @Headword {(ids is { Length: > 0 } ? "AND a.`DictionaryId` IN @Ids" : "")}
                    ORDER BY d.`Name`, a.`Id`";
        var rows = await connection.QueryAsync<DictionaryArticleRow>(new CommandDefinition(sql, new { Headword = normalisedHeadword, Ids = ids }, cancellationToken: cancellationToken));
        return rows.Select(r => r.ToModel()).ToList();
    }

    public async Task<IEnumerable<string>> SuggestHeadwords(string normalisedPrefix, IEnumerable<long> dictionaryIds, int limit, CancellationToken cancellationToken)
    {
        var ids = dictionaryIds?.ToArray();
        using var connection = connectionProvider.GetConnection();
        var sql = $@"SELECT a.`Headword` FROM DictionaryArticles a
                    WHERE a.`HeadwordNormalised` LIKE @Prefix {(ids is { Length: > 0 } ? "AND a.`DictionaryId` IN @Ids" : "")}
                    GROUP BY a.`Headword`
                    ORDER BY MIN(a.`HeadwordNormalised`), a.`Headword`
                    LIMIT @Limit";
        var prefix = normalisedPrefix.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
        return await connection.QueryAsync<string>(new CommandDefinition(sql, new { Prefix = prefix, Ids = ids, Limit = limit }, cancellationToken: cancellationToken));
    }

    private static object Parameters(DictionaryModel dictionary) => new
    {
        dictionary.Id,
        dictionary.Name,
        dictionary.Description,
        Format = dictionary.Format.ToString(),
        dictionary.SourceLanguage,
        dictionary.TargetLanguage,
        dictionary.EntryCount,
        dictionary.OwnerId,
        dictionary.OriginalFileName,
    };
}
