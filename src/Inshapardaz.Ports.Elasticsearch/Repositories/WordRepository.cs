using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories;
using Nest;

namespace Inshapardaz.Ports.Elasticsearch.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public WordRepository(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public async Task<Word> AddWord(int dictionaryId, Word word, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var count = await client.CountAsync<Word>(i => i.Index(index), cancellationToken);
            word.Id = count.Count + 1;
            word.DictionaryId = dictionaryId;

            await client.IndexAsync(word, i => i.Index(index).Type(DocumentTypes.Word), cancellationToken);

            return word;
        }

        public async Task DeleteWord(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);
            var existsResponse = await client.IndexExistsAsync(index, cancellationToken: cancellationToken);
            if (existsResponse.Exists)
            {
                await client.DeleteAsync<Word>(wordId,
                                                              i => i
                                                                   .Index(index)
                                                                   .Type(DocumentTypes.Word),
                                                              cancellationToken);
            }
        }

        public async Task UpdateWord(int dictionaryId, Word word, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            await client.UpdateAsync(DocumentPath<Word>.Id(word.Id),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(word), cancellationToken);
        }

        public async Task<Word> GetWordById(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var existsResponse = await client.IndexExistsAsync(index, cancellationToken: cancellationToken);
            if (!existsResponse.Exists)
            {
                return null;
            }

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            return response.Documents.SingleOrDefault();
        }

        public async Task<Word> GetWordByTitle(int dictionaryId, string title, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Title).Value(title)))
                                                                          )), cancellationToken);

            return response.Documents.FirstOrDefault();
        }

        public async Task<Page<Word>> GetWordsById(int dictionaryId, IEnumerable<long> wordIds, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .From(pageSize * (pageNumber - 1))
                                                               .Size(pageSize)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => AddIds(term.Field(f => f.Id), wordIds)))
                                                                          )), cancellationToken);

            var words = response.Documents;

            var count = response.HitsMetadata.Total;


            return new Page<Word>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = words
            };
        }

        public async Task<IEnumerable<Word>> GetWordsByTitles(int dictionaryId, IEnumerable<string> titles, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(100)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => AddValues(term.Field(f => f.Id), titles)))
                                                                          )), cancellationToken);

            return response.Documents;
        }

        public async Task<Page<Word>> GetWordsContaining(int dictionaryId, string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .From(pageSize * (pageNumber - 1))
                                                               .Size(pageSize)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(searchTerm)))
                                                                          )), cancellationToken);

            var words = response.Documents;

            var count = response.HitsMetadata.Hits.Count;

            return new Page<Word>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = words
            };
        }

        public async Task<Page<Word>> GetWords(int dictionaryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .From(pageSize * (pageNumber - 1))
                                                               .Size(pageSize)
                                                               .Query(q => q.MatchAll())
                                                          , cancellationToken);

            var words = response.Documents;

            var count = response.HitsMetadata.Total;

            return new Page<Word>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Data = words
            };
        }

        public async Task<int> GetWordCountByDictionary(int dictionaryId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.CountAsync<Word>(s => s
                                                               .Index(index)
                                                               .Query(q => q.MatchAll())
                                                          , cancellationToken);

            return (int) response.Count;
        }

        private TermQueryDescriptor<Word> AddIds(TermQueryDescriptor<Word> field, IEnumerable<long> queryDs)
        {
            foreach (var id in queryDs)
            {
                field.Value(id);
            }

            return field;
        }

        private TermQueryDescriptor<Word> AddValues(TermQueryDescriptor<Word> field, IEnumerable<string> queryDs)
        {
            foreach (var id in queryDs)
            {
                field.Value(id);
            }

            return field;
        }
    }
}
