using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Nest;

namespace Inshapardaz.Ports.Elasticsearch.Repositories
{
    public class MeaningRepository : IMeaningRepository
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public MeaningRepository(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }


        public async Task<Meaning> AddMeaning(int dictionaryId, long wordId, Meaning meaning, CancellationToken cancellationToken)
        {
            meaning.WordId = wordId;
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            var word = response.Documents.SingleOrDefault();

            if (word == null)
            {
                throw new NotFoundException();
            }

            meaning.Id = word.Meaning.Count + 1;
            word.Meaning.Add(meaning);

            await client.UpdateAsync(DocumentPath<Word>.Id(dictionaryId),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(word), cancellationToken);

            return meaning;
        }

        public async Task DeleteMeaning(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            var word = response.Documents.SingleOrDefault();

            if (word == null)
            {
                throw new NotFoundException();
            }

            var meaning = word.Meaning.SingleOrDefault(m => m.Id == meaningId);
            if (meaning != null)
            {
                word.Meaning.Remove(meaning);
                await client.UpdateAsync(DocumentPath<Word>.Id(dictionaryId),
                                         u => u
                                              .Index(index)
                                              .Type(DocumentTypes.Word)
                                              .DocAsUpsert()
                                              .Doc(word), cancellationToken);
            }
        }

        public async Task<Meaning> GetMeaningById(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            var word = response.Documents.SingleOrDefault();
            return word?.Meaning.SingleOrDefault(m => m.Id == meaningId);
        }

        public async Task UpdateMeaning(int dictionaryId, IFormattable wordId, Meaning meaning, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            var word = response.Documents.SingleOrDefault();

            if (word == null)
            {
                throw new NotFoundException();
            }

            var newMeaning = word.Meaning.SingleOrDefault(m => m.Id == meaning.Id);
            if (newMeaning == null)
            {
                throw new NotFoundException();
            }

            newMeaning.Context = meaning.Context;
            newMeaning.Value = meaning.Value;
            newMeaning.Example = meaning.Example;

            await client.UpdateAsync(DocumentPath<Word>.Id(dictionaryId),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(word), cancellationToken);
        }

        public async Task<IEnumerable<Meaning>> GetMeaningByContext(int dictionaryId, long wordId, string context, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            var word = response.Documents.SingleOrDefault();
            if (word != null)
            {
                if (string.IsNullOrWhiteSpace(context))
                {
                    return word.Meaning;
                }

                return word.Meaning.Where(m => m.Context == context);
            }

            return new Meaning[0];
        }

        public async Task<IEnumerable<Meaning>> GetMeaningByWordId(int dictionaryId, long wordId, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(1)
                                                               .Query(q => q
                                                                          .Bool(b => b
                                                                                    .Must(m => m
                                                                                              .Term(term => term.Field(f => f.Id).Value(wordId)))
                                                                          )), cancellationToken);

            var word = response.Documents.SingleOrDefault();
            return word?.Meaning;
        }
    }
}
