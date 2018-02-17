using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
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

        public Dictionary GetDictionary(int id)
        {
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));

            var response = client.Search<Dictionary>(s => s
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

        public void CreateDictionary(Dictionary dictionary)
        {
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));

            var response = client.Index(dictionary, i => i.Index(Indexes.Dictionaries).Type(DocumentTypes.Dictionary));

            if (!response.IsValid)
                throw new Exception(response.DebugInformation);
        }

        public void DeleteDictionary(int id)
        {
            var client = new ElasticClient(new Uri(_settings.ElasticsearchUrl));
            var response = client.Delete<Dictionary>(id, i => i
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
