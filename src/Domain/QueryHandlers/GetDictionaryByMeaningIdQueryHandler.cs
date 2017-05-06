using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByMeaningIdQueryHandler : AsyncQueryHandler<DictionaryByMeaningIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByMeaningIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public async override Task<Dictionary> ExecuteAsync(DictionaryByMeaningIdQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var meaning = await _database.Meanings.SingleOrDefaultAsync(m => m.Id == query.MeaningId);
            return meaning?.WordDetail.WordInstance.Dictionary;
        }
    }
}