using System;
using System.Threading;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class DictionaryDataHelpers
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public DictionaryDataHelpers(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public Domain.Entities.Dictionary GetDictionary(int id)
        {
            return _dictionaryRepository.GetDictionaryById(id, CancellationToken.None).Result;
        }

        public Domain.Entities.Dictionary CreateDictionary(Domain.Entities.Dictionary dictionary)
        {
            return _dictionaryRepository.AddDictionary(dictionary, CancellationToken.None).Result;
        }

        public void DeleteDictionary(int id)
        {
            if (_dictionaryRepository.GetDictionaryById(id, CancellationToken.None).Result != null)
            {
                _dictionaryRepository.DeleteDictionary(id, CancellationToken.None).Wait();
            }
        }
    }
}
