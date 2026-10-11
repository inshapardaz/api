using Dapper;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public class TransliterationEntryDto
    {
        public long Id { get; set; }
        public string Language { get; set; }
        public string Roman { get; set; }
        public string Native { get; set; }
        public string Meaning { get; set; }
        public string Category { get; set; }
    }

    public interface ITransliterationTestRepository
    {
        TransliterationEntryDto AddEntry(string language, string roman, string native);
        TransliterationEntryDto GetEntryByRoman(string language, string roman);
        void DeleteEntries(string language, IEnumerable<string> romans);
    }

    public class MySqlTransliterationTestRepository(IProvideConnection connectionProvider) : ITransliterationTestRepository
    {
        public TransliterationEntryDto AddEntry(string language, string roman, string native)
        {
            using var connection = connectionProvider.GetConnection();
            var entry = new TransliterationEntryDto { Language = language, Roman = roman, Native = native };
            entry.Id = connection.ExecuteScalar<long>(@"INSERT INTO TransliterationEntries (`Language`, `Roman`, `Native`) VALUES (@Language, @Roman, @Native);
                                                        SELECT LAST_INSERT_ID();", entry);
            return entry;
        }

        public TransliterationEntryDto GetEntryByRoman(string language, string roman)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.QueryFirstOrDefault<TransliterationEntryDto>("SELECT * FROM TransliterationEntries WHERE `Language` = @Language AND `Roman` = @Roman", new { Language = language, Roman = roman });
        }

        public void DeleteEntries(string language, IEnumerable<string> romans)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM TransliterationEntries WHERE `Language` = @Language AND `Roman` IN @Romans", new { Language = language, Romans = romans });
        }
    }

    public class SqlServerTransliterationTestRepository(IProvideConnection connectionProvider) : ITransliterationTestRepository
    {
        public TransliterationEntryDto AddEntry(string language, string roman, string native)
        {
            using var connection = connectionProvider.GetConnection();
            var entry = new TransliterationEntryDto { Language = language, Roman = roman, Native = native };
            entry.Id = connection.ExecuteScalar<long>(@"INSERT INTO [Language].TransliterationEntries ([Language], Roman, Native) OUTPUT Inserted.Id VALUES (@Language, @Roman, @Native)", entry);
            return entry;
        }

        public TransliterationEntryDto GetEntryByRoman(string language, string roman)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.QueryFirstOrDefault<TransliterationEntryDto>("SELECT * FROM [Language].TransliterationEntries WHERE [Language] = @Language AND Roman = @Roman", new { Language = language, Roman = roman });
        }

        public void DeleteEntries(string language, IEnumerable<string> romans)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM [Language].TransliterationEntries WHERE [Language] = @Language AND Roman IN @Romans", new { Language = language, Romans = romans });
        }
    }
}
