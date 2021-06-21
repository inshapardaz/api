using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Database.SqlServer.Repositories
{
    public class CorrectionRepository : ICorrectionRepository
    {
        private readonly IProvideConnection _connectionProvider;

        public CorrectionRepository(IProvideConnection connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<IEnumerable<Correction>> GetCorrectionForLanguage(string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT * FROM corrections WHERE language = @Language";
                var command = new CommandDefinition(sql, new
                {
                    Language = language
                }, cancellationToken: cancellationToken);

                return await connection.QueryAsync<Correction>(command);
            }
        }
    }
}
