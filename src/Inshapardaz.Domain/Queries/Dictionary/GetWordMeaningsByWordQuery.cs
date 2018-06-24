using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordMeaningsByWordQuery :  IQuery<IEnumerable<Meaning>>
    {
        public GetWordMeaningsByWordQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }

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