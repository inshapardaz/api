using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordMeaningsByContextQueryHandler : QueryHandlerAsync<GetWordMeaningsByContextQuery, IEnumerable<Meaning>>
    {
        private readonly IMeaningRepository _meaningRepository;

        public GetWordMeaningsByContextQueryHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }
        
        public override async Task<IEnumerable<Meaning>> ExecuteAsync(GetWordMeaningsByContextQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _meaningRepository.GetMeaningByContext(query.DictionaryId, query.WordId, query.Context, cancellationToken);
        }
    }
}