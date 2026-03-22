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
    }
}
