using Dapper;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Dictionary<string, string>> GetAutoCorrectionList(string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT IncorrectText, CorrectText FROM corrections WHERE language = @Language AND AutoCorrect = 1";
                var command = new CommandDefinition(sql, new
                {
                    Language = language
                }, cancellationToken: cancellationToken);

                var items = await connection.QueryAsync<(string key, string val)>(command);
                return items.ToDictionary(t => t.key, t => t.val); 
            }
        }

        public async Task<Dictionary<string, string>> GetPunctuationList(string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT IncorrectText, CorrectText FROM corrections WHERE language = @Language AND Punctuation = 1";
                var command = new CommandDefinition(sql, new
                {
                    Language = language
                }, cancellationToken: cancellationToken);

                var items = await connection.QueryAsync<(string key, string val)>(command);
                return items.ToDictionary(t => t.key, t => t.val);
            }
        }

        public async Task<Dictionary<string, string>> GetCorrectionList(string language, CancellationToken cancellationToken)
        {
            using (var connection = _connectionProvider.GetConnection())
            {
                var sql = @"SELECT IncorrectText, CorrectText FROM corrections WHERE language = @Language AND Punctuation = 0 AND Punctuation = 0";
                var command = new CommandDefinition(sql, new
                {
                    Language = language
                }, cancellationToken: cancellationToken);

                var items = await connection.QueryAsync<(string key, string val)>(command);
                return items.ToDictionary(t => t.key, t => t.val);
            }
        }
    }
}
