using System.Linq;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByIdQueryHandler : QueryHandlerAsync<GetDictionaryByIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(GetDictionaryByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var dictionary = await _database.Dictionary.SingleOrDefaultAsync(
                d => d.Id == query.DictionaryId, cancellationToken);
            
            return dictionary;
        }
    }
}