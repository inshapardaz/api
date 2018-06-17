using System.Threading;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class MeaningDataHelper
    {
        private readonly IMeaningRepository _meaningRepository;

        public MeaningDataHelper(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        public Domain.Entities.Meaning CreateMeaning(int dictionaryId, long wordId, Domain.Entities.Meaning meaning)
        {
            return _meaningRepository.AddMeaning(dictionaryId, wordId, meaning, CancellationToken.None).Result;
        }

        public Domain.Entities.Meaning GetMeaning(int dictionaryId, long wordId, long meaningId)
        {
            return _meaningRepository.GetMeaningById(dictionaryId, wordId, meaningId, CancellationToken.None).Result;
        }

        public void DeleteMeaning(int dictionaryId, long wordId, long meaningId)
        {
            _meaningRepository.DeleteMeaning(dictionaryId, wordId, meaningId, CancellationToken.None).Wait();
        }
    }
}