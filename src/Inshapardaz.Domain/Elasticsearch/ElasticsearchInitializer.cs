using System;
using Inshapardaz.Domain.Database.Entities;
using Nest;

namespace Inshapardaz.Domain.Elasticsearch
{
    public interface IInitialiseElasticSearch
    {
        void Initialise();
    }

    public class ElasticsearchInitializer : IInitialiseElasticSearch
    {
        private readonly IClientProvider _clientProvider;

        public ElasticsearchInitializer(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public void Initialise()
        {
            var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

            var indexConfig = new IndexState
            {
                Settings = settings
            };

            var client = _clientProvider.GetClient();

            if (!client.IndexExists(Indexes.Dictionaries).Exists)
            {
                client.CreateIndex(Indexes.Dictionaries, c => c
                                          .InitializeUsing(indexConfig)
                                          .Mappings(m => m.Map<Dictionary>(mp => mp.AutoMap()))
                                          .Mappings(m => m.Map<Word>(mp => mp.AutoMap()))
                                          .Mappings(m => m.Map<Meaning>(mp => mp.AutoMap()))
                                          .Mappings(m => m.Map<Translation>(mp => mp.AutoMap())));
                
            }

            client.PutIndexTemplate("dictionary-template",
                template => template
                    .Mappings(mappings => mappings
                        .Map<Word>("word", order => order
                            .Dynamic(DynamicMapping.Strict)
                            .AutoMap())
                    )
                );
        }
    }
}
