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
    public class GetWordMeaningsByContextQuery : IQuery<IEnumerable<Meaning>>
    {
        public GetWordMeaningsByContextQuery(int dictionaryId, long wordId, string context)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Context = context;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public string Context { get; }
    }

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