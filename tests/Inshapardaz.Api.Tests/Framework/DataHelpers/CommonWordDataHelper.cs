using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface ICommonWordsTestRepository
    {
        void AddCommonWord(CommonWordDto commonWord);
        void AddCommonWords(IEnumerable<CommonWordDto> commonWord);
        void DeleteCommonWords(IEnumerable<CommonWordDto> commonWords);
        CommonWordDto GetCommonWordById(long id);
        IEnumerable<string> GetWordsForLanguage(string language);
        void DeleteWordsForLanguage(string language);
        void DeleteSpellingReplacements(string language);
    }

    public class MySqlCommonWordsTestRepository(IProvideConnection connectionProvider) : ICommonWordsTestRepository
    {
        public void AddCommonWord(CommonWordDto commonWord)
        {
            using var connection = connectionProvider.GetConnection();
            var id = connection.ExecuteScalar<int>("INSERT INTO CommonWords (`Language`, `Word`) VALUES(@Language, @Word); SELECT LAST_INSERT_ID();", commonWord);
            commonWord.Id = id;
        }

        public void AddCommonWords(IEnumerable<CommonWordDto> commonWord)
        {
            foreach (var correction in commonWord)
            {
                AddCommonWord(correction);
            }
        }

        public void DeleteCommonWords(IEnumerable<CommonWordDto> commonWords)
        {
            using var connection = connectionProvider.GetConnection();
            var sql = "DELETE FROM CommonWords WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = commonWords.Select(a => a.Id) });
        }

        public CommonWordDto GetCommonWordById(long id)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.QuerySingleOrDefault<CommonWordDto>("SELECT * FROM CommonWords WHERE Id = @Id", new { Id = id });
        }
        public IEnumerable<string> GetWordsForLanguage(string language)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.Query<string>("SELECT `Word` FROM CommonWords WHERE `Language` = @Language", new { Language = language }).ToList();
        }

        public void DeleteWordsForLanguage(string language)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM CommonWords WHERE `Language` = @Language", new { Language = language });
        }

        public void DeleteSpellingReplacements(string language)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM SpellingReplacements WHERE `Language` = @Language", new { Language = language });
        }
    }

    public class SqlServerCommonWordsTestRepository(IProvideConnection connectionProvider) : ICommonWordsTestRepository
    {
        public void AddCommonWord(CommonWordDto commonWord)
        {
            using var connection = connectionProvider.GetConnection();
            var id = connection.ExecuteScalar<int>("INSERT INTO CommonWords(Language, Word) Output Inserted.Id VALUES(@Language, @Word)", commonWord);
            commonWord.Id = id;
        }

        public void AddCommonWords(IEnumerable<CommonWordDto> commonWord)
        {
            foreach (var correction in commonWord)
            {
                AddCommonWord(correction);
            }
        }

        public void DeleteCommonWords(IEnumerable<CommonWordDto> commonWords)
        {
            using var connection = connectionProvider.GetConnection();
            var sql = "DELETE FROM CommonWords WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = commonWords.Select(a => a.Id) });
        }

        public CommonWordDto GetCommonWordById(long id)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.QuerySingleOrDefault<CommonWordDto>("SELECT * FROM CommonWords WHERE Id = @Id", new { Id = id });
        }

        public IEnumerable<string> GetWordsForLanguage(string language)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.Query<string>("SELECT Word FROM [Library].CommonWords WHERE [Language] = @Language", new { Language = language }).ToList();
        }

        public void DeleteWordsForLanguage(string language)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM [Library].CommonWords WHERE [Language] = @Language", new { Language = language });
        }

        public void DeleteSpellingReplacements(string language)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM [Language].SpellingReplacements WHERE [Language] = @Language", new { Language = language });
        }
    }
}
