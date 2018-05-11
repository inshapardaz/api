using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
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