using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Ports.Elasticsearch.Repositories
{
    public class DictionaryRepository : IDictionaryRepository
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public DictionaryRepository(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public async Task<Dictionary> AddDictionary(Dictionary dictionary, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();

            var count = await client.CountAsync<Dictionary<,>>(s => s.Index(Indexes.Dictionaries), cancellationToken);
            dictionary.Id = (int)count.Count + 1;

            await client.IndexAsync(dictionary, i => i
                                                             .Index(Indexes.Dictionaries)
                                                             .Type(DocumentTypes.Dictionary),
                                    cancellationToken);

            var index = _indexProvider.GetIndexForDictionary(dictionary.Id);
            await client.CreateIndexAsync(index, cancellationToken: cancellationToken);

            return dictionary;
        }

        public Task<Dictionary> UpdateDictionary(int dictionaryId, Dictionary dictionary, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary> DeleteDictionary(int dictionaryId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dictionary>> GetAllDictionaries(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dictionary>> GetPublicDictionaries(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dictionary>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
