using System;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Nest;

namespace Inshapardaz.Ports.Elasticsearch
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
            Console.WriteLine("Initializing Mappings");
            var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

            var indexConfig = new IndexState
            {
                Settings = settings
            };

            var client = _clientProvider.GetClient();

            if (!client.IndexExists(Indexes.Dictionaries).Exists)
            {
                var result = client.CreateIndex(Indexes.Dictionaries, c => c
                                          .InitializeUsing(indexConfig)
                                          .Mappings(m => m.Map<Dictionary>(mp => mp.AutoMap()
                                                            .Properties(ps => ps
                                                                .Object<DictionaryDownload>( dd => dd.Name(n => n.Downloads).AutoMap())))
                                   ));
                if (!result.IsValid){
                    throw new System.Exception(result.DebugInformation);
                }

                var result2 = client.CreateIndex(Indexes.Dictionary, c => c
                                          .InitializeUsing(indexConfig)
                                          .Mappings(m => m.Map<Word>(mp => mp
                                                          .AutoMap()
                                                          .Properties(ps => ps
                                                            .Object<Meaning>(pm => pm.Name(n => n.Meaning).AutoMap())
                                                            .Object<Translation>(pt => pt.Name(n => n.Translation).AutoMap())
                                                            .Object<WordRelation>(pr => pr.Name(n => n.Relations).AutoMap())
                                    ))));
                
                if (!result2.IsValid){
                    throw new System.Exception(result2.DebugInformation);
                }
            }

            var result3 = client.PutIndexTemplate("dictionary-template",
                template => template
                    .IndexPatterns("dictionary-*")
                    .Mappings(mappings => mappings
                        .Map<Word>("word", order => order
                            .AutoMap())
                    )
                );
            if (!result3.IsValid){
                throw new System.Exception(result3.DebugInformation);
            }
        }
    }
}
