using System.Linq;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByIdQueryHandler : QueryHandlerAsync<DictionaryByIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(DictionaryByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = _database.Dictionary.Where(d => d.Id == query.DictionaryId);
            return await result.SingleOrDefaultAsync(cancellationToken);
        }
    }
}