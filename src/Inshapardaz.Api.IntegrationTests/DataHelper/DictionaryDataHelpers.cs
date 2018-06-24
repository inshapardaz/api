using System;
using System.Threading;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class DictionaryDataHelpers
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public DictionaryDataHelpers(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public Domain.Entities.Dictionary.Dictionary GetDictionary(int id)
        {
            return _dictionaryRepository.GetDictionaryById(id, CancellationToken.None).Result;
        }

        public Domain.Entities.Dictionary.Dictionary CreateDictionary(Domain.Entities.Dictionary.Dictionary dictionary)
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
