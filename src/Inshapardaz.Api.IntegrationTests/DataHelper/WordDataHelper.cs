using System;
using System.Threading;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class WordDataHelper
    {
        private readonly IWordRepository _wordRepository;

        public WordDataHelper(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        public Word GetWord(int dictionaryId, long wordId)
        {
            return _wordRepository.GetWordById(dictionaryId, wordId, CancellationToken.None).Result;
        }

        public Word CreateWord(int dictionaryId, Word word)
        {
            return _wordRepository.AddWord(dictionaryId, word, CancellationToken.None).Result;
        }

        public void DeleteWord(int dictionaryId, long wordId)
        {
            _wordRepository.DeleteWord(dictionaryId, wordId, CancellationToken.None).Wait();
        }
    }
}
