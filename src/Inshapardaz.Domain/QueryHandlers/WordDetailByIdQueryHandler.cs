using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordDetailByIdQueryHandler : QueryHandlerAsync<WordDetailByIdQuery, WordDetail>
    {
        private readonly IDatabaseContext _database;

        public WordDetailByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<WordDetail> ExecuteAsync(WordDetailByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.WordDetail.SingleOrDefaultAsync(w => w.Id == query.Id, cancellationToken);
        }
    }
}