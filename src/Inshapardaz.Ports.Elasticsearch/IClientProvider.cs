using System;
using Inshapardaz.Domain;
using Nest;

namespace Inshapardaz.Ports.Elasticsearch
{
    public interface IClientProvider
    {
        IElasticClient GetClient();
    }

    public class ClientProvider : IClientProvider
    {
        private readonly Settings _settings;

        public ClientProvider(Settings settings)
        {
            _settings = settings;
        }
        public IElasticClient GetClient()
        {
            return new ElasticClient(new Uri(_settings.ElasticsearchUrl));
        }
    }
}
