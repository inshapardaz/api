using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordMeaningsByWordQueryHandler : QueryHandlerAsync<GetWordMeaningsByWordQuery, IEnumerable<Meaning>>
    {
        private readonly IMeaningRepository _meaningRepository;

        public GetWordMeaningsByWordQueryHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        public override async Task<IEnumerable<Meaning>> ExecuteAsync(GetWordMeaningsByWordQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _meaningRepository.GetMeaningByWordId(query.DictionaryId, query.WordId, cancellationToken);
        }
    }
}