using Dapper;
using Inshapardaz.Api.Tests.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.DataHelpers
{
    public static class CorrectionDataHelper
    {
        public static void AddCorrection(this IDbConnection connection, CorrectionDto correction)
        {
            var id = connection.ExecuteScalar<int>("Insert Into Corrections(Language, Profile, IncorrectText, CorrectText, CompleteWord) Output Inserted.Id VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord)", correction);
            correction.Id = id;
        }

        public static void AddCorrections(this IDbConnection connection, IEnumerable<CorrectionDto> corrections)
        {
            foreach (var correction in corrections)
            {
                AddCorrection(connection, correction);
            }
        }

        public static void DeleteCorrections(this IDbConnection connection, IEnumerable<CorrectionDto> corrections)
        {
            var sql = "Delete From Corrections Where Id IN @Ids";
            connection.Execute(sql, new { Ids = corrections.Select(a => a.Id) });
        }

        public static CorrectionDto GetCorrectionById(this IDbConnection connection, long id)
        {
            return connection.QuerySingleOrDefault<CorrectionDto>("Select * From Corrections Where Id = @Id", new { Id = id });
        }
    }
}
