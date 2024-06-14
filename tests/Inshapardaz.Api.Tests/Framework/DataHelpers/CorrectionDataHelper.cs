using Dapper;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public static class CorrectionDataHelper
    {
        private static DatabaseTypes _dbType => TestBase.DatabaseType;

        public static void AddCorrection(this IDbConnection connection, CorrectionDto correction)
        {
            var id = _dbType == DatabaseTypes.SqlServer 
                ? connection.ExecuteScalar<int>("INSERT INTO Corrections(Language, Profile, IncorrectText, CorrectText, CompleteWord) Output Inserted.Id VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord)", correction)
                : connection.ExecuteScalar<int>("INSERT INTO Corrections(`Language`, `Profile`, IncorrectText, CorrectText, CompleteWord) VALUES(@Language, @Profile, @IncorrectText, @CorrectText, @CompleteWord); SELECT LAST_INSERT_ID();", correction);
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
            var sql = "DELETE FROM Corrections WHERE Id IN @Ids";
            connection.Execute(sql, new { Ids = corrections.Select(a => a.Id) });
        }

        public static CorrectionDto GetCorrectionById(this IDbConnection connection, long id)
        {
            return connection.QuerySingleOrDefault<CorrectionDto>("SELECT * FROM Corrections WHERE Id = @Id", new { Id = id });
        }
    }
}
