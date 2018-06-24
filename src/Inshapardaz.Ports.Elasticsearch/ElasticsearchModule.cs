using System;
using AutoMapper;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Inshapardaz.Ports.Elasticsearch.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Ports.Elasticsearch
{

    public static class ElasticsearchModule
    {
        public static Profile GetMappingProfile()
        {
            return new MappingProfile();
        }

        public static void AddElasticSearch(IApplicationBuilder app)
        {
            var initialiser = app.ApplicationServices.GetService<IInitialiseElasticSearch>();
            initialiser.Initialise();
        }
        public static void ConfigureElasticsearch(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDictionaryRepository, DictionaryRepository>();

            services.AddTransient<IClientProvider, ClientProvider>();
            services.AddTransient<IInitialiseElasticSearch, ElasticsearchInitializer>();
            services.AddTransient<IProvideIndex, IndexProvider>();
        }
    }
}
