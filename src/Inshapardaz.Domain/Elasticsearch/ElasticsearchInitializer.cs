using Inshapardaz.Domain.Entities;
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
                                          .Mappings(m => m.Map<Dictionary>(mp => mp.AutoMap()
                                                            .Properties(ps => ps
                                                                .Object<DictionaryDownload>( dd => dd.Name(n => n.Downloads).AutoMap())))
                                   ));
                client.CreateIndex(Indexes.Dictionary, c => c
                                          .InitializeUsing(indexConfig)
                                          .Mappings(m => m.Map<Word>(mp => mp
                                                          .AutoMap()
                                                          .Properties(ps => ps
                                                            .Object<Meaning>(pm => pm.Name(n => n.Meaning).AutoMap())
                                                            .Object<Translation>(pt => pt.Name(n => n.Translation).AutoMap())
                                                            .Object<WordRelation>(pr => pr.Name(n => n.Relations).AutoMap())
                                    ))));
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
