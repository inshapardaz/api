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
    }

    public class MySqlCorrectionTestRepository(IProvideConnection connectionProvider) : ICorrectionTestRepository
    {
        public void AddCorrection(CorrectionDto correction)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Corrections(`Language`, `Profile`, IncorrectText, CorrectText, CompleteWord) VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord); SELECT LAST_INSERT_ID();", correction);
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
    }

    public class SqlServerCorrectionTestRepository(IProvideConnection connectionProvider) : ICorrectionTestRepository
    {
        public void AddCorrection(CorrectionDto correction)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                var id = connection.ExecuteScalar<int>("INSERT INTO Corrections(Language, Profile, IncorrectText, CorrectText, CompleteWord) Output Inserted.Id VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord)", correction);
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
    }
}
