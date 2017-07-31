using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByWordDetailIdQueryHandler : AsyncQueryHandler<DictionaryByWordDetailIdQuery,
        Model.Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByWordDetailIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(DictionaryByWordDetailIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _database.WordDetail.SingleOrDefaultAsync(wd => wd.Id == query.WordDetailId,
                cancellationToken);
            return word?.WordInstance.Dictionary;
        }
    }
}