using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordMeaningByIdQueryHandler : QueryHandlerAsync<GetWordMeaningByIdQuery, Meaning>
    {
        private readonly IDatabaseContext _database;

        public GetWordMeaningByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Meaning> ExecuteAsync(GetWordMeaningByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Meaning.SingleOrDefaultAsync(t => t.Id == query.MeaningId, cancellationToken);
        }
    }
}