using System;
using System.Linq;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Elasticsearch;
using Nest;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class WordDataHelper
    {
        private readonly Settings _settings;
        private readonly IProvideIndex _indexProvider;

        public WordDataHelper(Settings settings, IProvideIndex indexProvider)
        {
            _settings = settings;
            _indexProvider = indexProvider;
        }

        public Domain.Entities.Word GetWord(int dictionaryId, long wordId)
        {
            if (dictionaryId > 0) throw new Exception("Non test data cannot be manipulated using this helper");
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));
            var index = _indexProvider.GetIndexForDictionary(dictionaryId);
            var response = client.Search<Domain.Entities.Word>(s => s
                                .Index(index)
                                .Size(1)
                                .Query(q => q
                                    .Bool(b => b
                                        .Must(m => m
                                            .Term(term => term.Field(f => f.Id).Value(wordId)))
                                )));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);

            return response.Documents.FirstOrDefault();
        }

        public void CreateWord(int dictionaryId, Domain.Entities.Word word)
        {
            if (dictionaryId > 0) throw new Exception("Non test data cannot be manipulated using this helper");

            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));

            var index = _indexProvider.GetIndexForDictionary(dictionaryId);
            word.DictionaryId = dictionaryId;

            var response = client.Index(word, i => i.Index(index).Type(DocumentTypes.Word));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }

        public void DeleteWord(int dictionaryId, long wordId)
        {
            if (dictionaryId > 0) throw new Exception("Non test data cannot be manipulated using this helper");

            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));

            var index = _indexProvider.GetIndexForDictionary(dictionaryId);

            var response = client.Delete<Domain.Entities.Word>(wordId, i => i
                                .Index(index)
                                .Type(DocumentTypes.Word));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }

        public void RefreshIndex()
        {
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));
            client.Refresh(Indices.All);
        }
    }
}
