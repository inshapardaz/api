using System.Threading;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class MeaningDataHelper
    {
        private readonly IMeaningRepository _meaningRepository;

        public MeaningDataHelper(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        public Meaning CreateMeaning(int dictionaryId, long wordId, Meaning meaning)
        {
            return _meaningRepository.AddMeaning(dictionaryId, wordId, meaning, CancellationToken.None).Result;
        }

        public Meaning GetMeaning(int dictionaryId, long wordId, long meaningId)
        {
            return _meaningRepository.GetMeaningById(dictionaryId, wordId, meaningId, CancellationToken.None).Result;
        }

        public void DeleteMeaning(int dictionaryId, long wordId, long meaningId)
        {
            _meaningRepository.DeleteMeaning(dictionaryId, wordId, meaningId, CancellationToken.None).Wait();
        }
    }
}