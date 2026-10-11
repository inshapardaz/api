using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface IDictionaryTestRepository
    {
        void AddDictionary(DictionaryDto dictionary);
        void AddArticle(DictionaryArticleDto article);
        DictionaryDto GetDictionaryById(long id);
        IEnumerable<DictionaryArticleDto> GetArticles(long dictionaryId);
        void DeleteDictionary(long id);
    }

    public class MySqlDictionaryTestRepository(IProvideConnection connectionProvider) : IDictionaryTestRepository
    {
        public void AddDictionary(DictionaryDto dictionary)
        {
            using var connection = connectionProvider.GetConnection();
            var sql = @"INSERT INTO Dictionaries (`Name`, `Description`, `Format`, `SourceLanguage`, `TargetLanguage`, `EntryCount`, `OwnerId`, `OriginalFileName`, `DateCreated`)
                        VALUES (@Name, @Description, @Format, @SourceLanguage, @TargetLanguage, @EntryCount, @OwnerId, @OriginalFileName, @DateCreated);
                        SELECT LAST_INSERT_ID();";
            dictionary.Id = connection.ExecuteScalar<long>(sql, dictionary);
        }

        public void AddArticle(DictionaryArticleDto article)
        {
            using var connection = connectionProvider.GetConnection();
            var sql = @"INSERT INTO DictionaryArticles (`DictionaryId`, `Headword`, `HeadwordNormalised`, `Pronunciation`, `Senses`, `Links`, `RawDefinition`, `Alternates`)
                        VALUES (@DictionaryId, @Headword, @HeadwordNormalised, @Pronunciation, @Senses, @Links, @RawDefinition, @Alternates);
                        SELECT LAST_INSERT_ID();";
            article.Id = connection.ExecuteScalar<long>(sql, article);
        }

        public DictionaryDto GetDictionaryById(long id)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.QuerySingleOrDefault<DictionaryDto>("SELECT * FROM Dictionaries WHERE `Id` = @Id", new { Id = id });
        }

        public IEnumerable<DictionaryArticleDto> GetArticles(long dictionaryId)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.Query<DictionaryArticleDto>("SELECT * FROM DictionaryArticles WHERE `DictionaryId` = @Id ORDER BY `Id`", new { Id = dictionaryId }).ToList();
        }

        public void DeleteDictionary(long id)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM Dictionaries WHERE `Id` = @Id", new { Id = id });
        }
    }

    public class SqlServerDictionaryTestRepository(IProvideConnection connectionProvider) : IDictionaryTestRepository
    {
        public void AddDictionary(DictionaryDto dictionary)
        {
            using var connection = connectionProvider.GetConnection();
            var sql = @"INSERT INTO [Language].Dictionaries ([Name], [Description], [Format], SourceLanguage, TargetLanguage, EntryCount, OwnerId, OriginalFileName, DateCreated)
                        OUTPUT Inserted.Id
                        VALUES (@Name, @Description, @Format, @SourceLanguage, @TargetLanguage, @EntryCount, @OwnerId, @OriginalFileName, @DateCreated)";
            dictionary.Id = connection.ExecuteScalar<long>(sql, dictionary);
        }

        public void AddArticle(DictionaryArticleDto article)
        {
            using var connection = connectionProvider.GetConnection();
            var sql = @"INSERT INTO [Language].DictionaryArticles (DictionaryId, Headword, HeadwordNormalised, Pronunciation, Senses, Links, RawDefinition, Alternates)
                        OUTPUT Inserted.Id
                        VALUES (@DictionaryId, @Headword, @HeadwordNormalised, @Pronunciation, @Senses, @Links, @RawDefinition, @Alternates)";
            article.Id = connection.ExecuteScalar<long>(sql, article);
        }

        public DictionaryDto GetDictionaryById(long id)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.QuerySingleOrDefault<DictionaryDto>("SELECT * FROM [Language].Dictionaries WHERE Id = @Id", new { Id = id });
        }

        public IEnumerable<DictionaryArticleDto> GetArticles(long dictionaryId)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.Query<DictionaryArticleDto>("SELECT * FROM [Language].DictionaryArticles WHERE DictionaryId = @Id ORDER BY Id", new { Id = dictionaryId }).ToList();
        }

        public void DeleteDictionary(long id)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM [Language].Dictionaries WHERE Id = @Id", new { Id = id });
        }
    }
}
