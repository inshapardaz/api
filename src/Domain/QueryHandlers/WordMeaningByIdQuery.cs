using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordMeaningByIdQueryHandler : AsyncQueryHandler<WordMeaningByIdQuery, Meaning>
    {
        private readonly IDatabaseContext _database;

        public WordMeaningByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Meaning> ExecuteAsync(WordMeaningByIdQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Meanings.SingleOrDefaultAsync(t => t.Id == query.Id);
        }
    }
}