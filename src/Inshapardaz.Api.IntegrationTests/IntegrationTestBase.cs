using System;
using System.IO;
using System.Net.Http;
using Inshapardaz.Api.IntegrationTests.DataHelper;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Elasticsearch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Api.IntegrationTests
{
    public abstract class IntegrationTestBase : IDisposable
    {
        private static readonly TestServer TestServer;
        private HttpClient _authenticatedClient;
        private HttpClient _client;

        static  IntegrationTestBase()
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false)
                                .Build();
            TestServer = new TestServer(new WebHostBuilder()
                                        .UseConfiguration(configuration)
                                             .UseStartup<TestStartup>());
        }

        protected Settings Settings => TestServer.Host.Services.GetService<Settings>();

        protected DictionaryDataHelpers DictionaryDataHelper => new DictionaryDataHelpers(Settings);
        protected WordDataHelper WordDataHelper => new WordDataHelper(Settings, TestServer.Host.Services.GetService<IProvideIndex>());

        protected HttpResponseMessage Response;

        protected HttpClient GetClient()
        {
            _client?.Dispose();
            _client = TestServer.CreateClient();
            return _client;
        }

        protected HttpClient GetAuthenticatedClient(Guid? userGuid, string userName = "integration.user")
        {
            _authenticatedClient?.Dispose();
            _authenticatedClient = TestServer.CreateClient();
            _authenticatedClient.DefaultRequestHeaders.Add(AuthenticatedTestRequestMiddleware.TestingHeader, AuthenticatedTestRequestMiddleware.TestingHeaderValue);
            _authenticatedClient.DefaultRequestHeaders.Add("my-name", userName);
            _authenticatedClient.DefaultRequestHeaders.Add("my-id", (userGuid ?? Guid.NewGuid()).ToString());
            return _authenticatedClient;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Response?.Dispose();
                _authenticatedClient?.Dispose();
                _client?.Dispose();
            }
        }
    }
}
