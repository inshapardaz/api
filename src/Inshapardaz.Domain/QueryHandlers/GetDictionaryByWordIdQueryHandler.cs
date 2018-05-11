using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    // REMOVE IT : 
    public class GetDictionaryByWordIdQueryHandler : QueryHandlerAsync<DictionaryByWordIdQuery, Dictionary>
    {
        //private readonly IDatabaseContext _database;

        //public GetDictionaryByWordIdQueryHandler(IDatabaseContext database)
        //{
        //    _database = database;
        //}

        //public override async Task<Dictionary> ExecuteAsync(DictionaryByWordIdQuery query,
        //    CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var word = await _database.Word
        //                              .Include(w => w.Dictionary)
        //                              .SingleOrDefaultAsync(wd => wd.Id == query.WordId, cancellationToken);
        //    return word?.Dictionary;
        //}

        public override Task<Dictionary> ExecuteAsync(DictionaryByWordIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}