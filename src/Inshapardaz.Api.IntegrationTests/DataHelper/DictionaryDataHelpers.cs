using System;
using System.Linq;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Elasticsearch;
using Nest;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class DictionaryDataHelpers
    {
        private readonly Settings _settings;

        public DictionaryDataHelpers(Settings settings)
        {
            _settings = settings;
        }

        public Domain.Entities.Dictionary GetDictionary(int id)
        {
            if (id > 0) throw new Exception("Non test data cannot be manipulated using this helper");
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));

            var response = client.Search<Domain.Entities.Dictionary>(s => s
                                 .Index(Indexes.Dictionaries)
                                 .Size(1)
                                 .Query(q => q
                                 .Bool(b => b
                                     .Must(m => m
                                     .Term(term => term.Field(f => f.Id).Value(id)))
                                )));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);

            return response.Documents.FirstOrDefault();
        }

        public void CreateDictionary(Domain.Entities.Dictionary dictionary)
        {
            if (dictionary.Id > 0) throw new Exception("Non test data cannot be manipulated using this helper");
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));

            var response = client.Index(dictionary, i => i.Index(Indexes.Dictionaries).Type(DocumentTypes.Dictionary));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }

        public void DeleteDictionary(int id)
        {
            if (id > 0) throw new Exception("Non test data cannot be manipulated using this helper");
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));
            var response = client.Delete<Domain.Entities.Dictionary>(id, i => i
                                            .Index(Indexes.Dictionaries)
                                            .Type(DocumentTypes.Dictionary));

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
