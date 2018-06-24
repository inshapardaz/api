using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetTranslationByIdQuery : IQuery<Translation>
    {
        public GetTranslationByIdQuery(int dictionaryId, long wordId, long translationId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            TranslationId = translationId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long TranslationId { get; }
    }

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