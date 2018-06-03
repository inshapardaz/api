using System;
using System.Threading;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class WordDataHelper
    {
        private readonly IWordRepository _wordRepository;

        public WordDataHelper(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public Domain.Entities.Word GetWord(int dictionaryId, long wordId)
        {
            return _wordRepository.GetWordById(dictionaryId, wordId, CancellationToken.None).Result;
        }

        public Domain.Entities.Word CreateWord(int dictionaryId, Domain.Entities.Word word)
        {
            return _wordRepository.AddWord(dictionaryId, word, CancellationToken.None).Result;
        }

        public void DeleteWord(int dictionaryId, long wordId)
        {
            _wordRepository.DeleteWord(dictionaryId, wordId, CancellationToken.None).Wait();
        }
    }
}
