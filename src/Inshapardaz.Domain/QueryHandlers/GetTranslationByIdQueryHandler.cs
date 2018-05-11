using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationByIdQueryHandler : QueryHandlerAsync<GetTranslationByIdQuery, Translation>
    {
        private readonly ITranslationRepository _translationRepository;

        public GetTranslationByIdQueryHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public override async Task<Translation> ExecuteAsync(GetTranslationByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _translationRepository.GetTranslationById(query.DictionaryId, query.WordId, query.TranslationId, cancellationToken);
        }
    }
}