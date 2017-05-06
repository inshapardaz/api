using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByWordIdQueryHandler : AsyncQueryHandler<DictionaryByWordIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }
        
        public async override Task<Dictionary> ExecuteAsync(DictionaryByWordIdQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _database.Words.SingleOrDefaultAsync(wd => wd.Id == query.WordId);
            return word?.Dictionary;
        }
    }
}
