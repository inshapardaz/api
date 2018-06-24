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
    public class GetTranslationsByWordIdQuery : IQuery<IEnumerable<Translation>>
    {
        public GetTranslationsByWordIdQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }

    public class GetTranslationsByWordIdQueryHandler : QueryHandlerAsync<GetTranslationsByWordIdQuery,
        IEnumerable<Translation>>
    {
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationsByWordIdQueryHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public override async Task<IEnumerable<Translation>> ExecuteAsync(GetTranslationsByWordIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _translationRepository.GetWordTranslations(query.DictionaryId, query.WordId, cancellationToken);
        }
    }
}