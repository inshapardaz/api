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
    public class TranslationRepository : ITranslationRepository
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public TranslationRepository(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public async Task<Translation> AddTranslation(int dictionaryId, long wordId, Translation translation, CancellationToken cancellationToken)
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

            translation.Id = word.Translation.Count;
            word.Translation.Add(translation);

            await client.UpdateAsync(DocumentPath<Word>.Id(dictionaryId),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(word), cancellationToken);
            return translation;
        }

        public async Task DeleteTranslation(int dictionaryId, long wordId, long translationId, CancellationToken cancellationToken)
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

            var translation = word.Translation.SingleOrDefault(m => m.Id == translationId);
            if (translation != null)
            {
                word.Translation.Remove(translation);
                await client.UpdateAsync(DocumentPath<Word>.Id(dictionaryId),
                                         u => u
                                              .Index(index)
                                              .Type(DocumentTypes.Word)
                                              .DocAsUpsert()
                                              .Doc(word), cancellationToken);
            }
        }

        public async Task UpdateTranslation(int dictionaryId, long wordId, Translation translation, CancellationToken cancellationToken)
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

            var newTranslation = word.Translation.SingleOrDefault(m => m.Id == translation.Id);
            if (newTranslation == null)
            {
                throw new NotFoundException();
            }

            newTranslation.Language = translation.Language;
            newTranslation.Value = translation.Value;
            newTranslation.IsTrasnpiling = translation.IsTrasnpiling;

            await client.UpdateAsync(DocumentPath<Word>.Id(dictionaryId),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(word), cancellationToken);
        }

        public async Task<Translation> GetTranslationById(int dictionaryId, long wordId, long translationId, CancellationToken cancellationToken)
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

            return word?.Translation.SingleOrDefault(t => t.Id == translationId);
        }

        public async Task<IEnumerable<Translation>> GetWordTranslationsByLanguage(int dictionaryId, long wordId, Languages language, CancellationToken cancellationToken)
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

            return word?.Translation.Where(t => t.Language == language);
        }

        public async Task<IEnumerable<Translation>> GetWordTranslations(int dictionaryId, long wordId, CancellationToken cancellationToken)
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
            return word?.Translation;
        }
    }
}
