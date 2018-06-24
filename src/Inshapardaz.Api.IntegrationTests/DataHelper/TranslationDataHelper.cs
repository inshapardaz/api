using System.Threading;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class TranslationDataHelper
    {
        private readonly ITranslationRepository _translationRepository;

        public TranslationDataHelper(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }

        public Translation CreateTranslation(int dictionaryId, long wordId, Translation translation)
        {
            return _translationRepository.AddTranslation(dictionaryId, wordId, translation, CancellationToken.None).Result;
        }

        public Translation GetTranslation(int dictionaryId, long wordId, long meaningId)
        {
            return _translationRepository.GetTranslationById(dictionaryId, wordId, meaningId, CancellationToken.None).Result;
        }

        public void DeleteTranslation(int dictionaryId, long wordId, long meaningId)
        {
            _translationRepository.DeleteTranslation(dictionaryId, wordId, meaningId, CancellationToken.None).Wait();
        }
    }
}