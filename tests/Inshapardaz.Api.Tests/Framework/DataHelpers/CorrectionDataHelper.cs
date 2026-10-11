using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public interface ICorrectionTestRepository
    {
        void AddCorrection(CorrectionDto correction);
        void AddCorrections(IEnumerable<CorrectionDto> corrections);
        void DeleteCorrections(IEnumerable<CorrectionDto> corrections);
        CorrectionDto GetCorrectionById(long id);
        IEnumerable<CorrectionDto> GetCorrections(string language, string profile);
        void DeleteCorrectionsForLanguage(string language);
    }

    public class MySqlCorrectionTestRepository(IProvideConnection connectionProvider) : ICorrectionTestRepository
    {
        public void AddCorrection(CorrectionDto correction)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Corrections(`Language`, `Profile`, IncorrectText, CorrectText, CompleteWord, `Usage`) VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord, @Usage); SELECT LAST_INSERT_ID();", correction);
                correction.Id = id;
            }
        }

        public void AddCorrections(IEnumerable<CorrectionDto> corrections)
        {
            foreach (var correction in corrections)
            {
                AddCorrection(correction);
            }
        }

        public void DeleteCorrections(IEnumerable<CorrectionDto> corrections)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Corrections WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = corrections.Select(a => a.Id) });
            }
        }

        public CorrectionDto GetCorrectionById(long id)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<CorrectionDto>("SELECT * FROM Corrections WHERE Id = @Id", new { Id = id });
            }
        }

        public IEnumerable<CorrectionDto> GetCorrections(string language, string profile)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.Query<CorrectionDto>("SELECT * FROM Corrections WHERE Language = @Language AND Profile = @Profile ORDER BY Id", new { Language = language, Profile = profile }).ToList();
        }

        public void DeleteCorrectionsForLanguage(string language)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM Corrections WHERE Language = @Language", new { Language = language });
        }
    }

    public class SqlServerCorrectionTestRepository(IProvideConnection connectionProvider) : ICorrectionTestRepository
    {
        public void AddCorrection(CorrectionDto correction)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Corrections(Language, Profile, IncorrectText, CorrectText, CompleteWord, Usage) Output Inserted.Id VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord, @Usage)", correction);
                correction.Id = id;
            }
        }

        public void AddCorrections(IEnumerable<CorrectionDto> corrections)
        {
            foreach (var correction in corrections)
            {
                AddCorrection(correction);
            }
        }

        public void DeleteCorrections(IEnumerable<CorrectionDto> corrections)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                var sql = "DELETE FROM Corrections WHERE Id IN @Ids";
                connection.Execute(sql, new { Ids = corrections.Select(a => a.Id) });
            }
        }

        public CorrectionDto GetCorrectionById(long id)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                return connection.QuerySingleOrDefault<CorrectionDto>("SELECT * FROM Corrections WHERE Id = @Id", new { Id = id });
            }
        }

        public IEnumerable<CorrectionDto> GetCorrections(string language, string profile)
        {
            using var connection = connectionProvider.GetConnection();
            return connection.Query<CorrectionDto>("SELECT * FROM Corrections WHERE Language = @Language AND Profile = @Profile ORDER BY Id", new { Language = language, Profile = profile }).ToList();
        }

        public void DeleteCorrectionsForLanguage(string language)
        {
            using var connection = connectionProvider.GetConnection();
            connection.Execute("DELETE FROM Corrections WHERE Language = @Language", new { Language = language });
        }
    }
}
