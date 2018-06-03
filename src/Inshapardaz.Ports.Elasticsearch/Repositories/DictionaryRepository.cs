using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories;
using Nest;

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

            var count = await client.CountAsync<Dictionary>(s => s.Index(Indexes.Dictionaries), cancellationToken);
            dictionary.Id = (int) (count.Count + 1);

            await client.IndexAsync(dictionary, i => i
                                                             .Index(Indexes.Dictionaries)
                                                             .Type(DocumentTypes.Dictionary),
                                    cancellationToken);

            var index = _indexProvider.GetIndexForDictionary(dictionary.Id);
            await client.CreateIndexAsync(index, cancellationToken: cancellationToken);

            return dictionary;
        }

        public async Task UpdateDictionary(int dictionaryId, Dictionary dictionary, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            await client.UpdateAsync(DocumentPath<Dictionary>.Id(dictionaryId),
                                     u => u
                                          .Index(Indexes.Dictionaries)
                                          .Type(DocumentTypes.Dictionary)
                                          .DocAsUpsert()
                                          .Doc(dictionary), cancellationToken);
        }

        public async Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            await client.DeleteAsync<Dictionary>(dictionaryId,
                                                 i => i
                                                      .Index(Indexes.Dictionaries)
                                                      .Type(DocumentTypes.Dictionary),
                                                 cancellationToken);

            var index = _indexProvider.GetIndexForDictionary(dictionaryId);
            await client.DeleteIndexAsync(index, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Dictionary>> GetAllDictionaries(CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var response = await client.SearchAsync<Dictionary>(s => s
                                                                     .Index(Indexes.Dictionaries)
                                                                     .Size(100)
                                                                     .Query(q => q), cancellationToken);
            return response.Documents;
        }

        public async Task<IEnumerable<Dictionary>> GetPublicDictionaries(CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var response = await client.SearchAsync<Dictionary>(s => s
                                                                 .Index(Indexes.Dictionaries)
                                                                 .Size(100)
                                                                 .Query(q => q
                                                                                 .Term(p => p.IsPublic, true)
                                                                 ), cancellationToken);
            return response.Documents;
        }

        public async Task<IEnumerable<Dictionary>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var response = await client.SearchAsync<Dictionary>(s => s
                                                                 .Index(Indexes.Dictionaries)
                                                                 .Size(100)
                                                                 .Query(q => q.Term(p => p.UserId, userId)
                                                                 ), cancellationToken);

            return response.Documents;
        }

        public async Task<Dictionary> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();

            ISearchResponse<Dictionary> response =
                await client.SearchAsync<Dictionary>(s => s
                                                          .Index(Indexes.Dictionaries)
                                                          .Size(1)
                                                          .Query(q => q
                                                                     .Bool(b => b
                                                                               .Must(m => m
                                                                                         .Term(term => term.Field(f => f.Id).Value(dictionaryId)))
                                                                     )), cancellationToken);

            return response.Documents.SingleOrDefault();

        }
    }
}
