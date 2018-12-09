using System;
using System.IO;
using System.Net.Http;
using Inshapardaz.Api.IntegrationTests.DataHelper;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Repositories.Dictionary;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Ports.Database;
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
                                .AddJsonFile("testsettings.json", optional: false)
                                .Build();
            TestServer = new TestServer(new WebHostBuilder()
                                        .UseConfiguration(configuration)
                                        .UseEnvironment("Test")
                                        .UseStartup<Startup>());

            TestServer.Host.Services.GetService<IDatabaseContext>().Database.EnsureCreated();
        }

        protected Settings Settings => TestServer.Host.Services.GetService<Settings>();

        protected DictionaryDataHelpers DictionaryDataHelper => new DictionaryDataHelpers(TestServer.Host.Services.GetService<IDictionaryRepository>());

        protected WordDataHelper WordDataHelper => new WordDataHelper(TestServer.Host.Services.GetService<IWordRepository>());

        protected MeaningDataHelper MeaningDataHelper => new MeaningDataHelper(TestServer.Host.Services.GetService<IMeaningRepository>());

        protected TranslationDataHelper TranslationDataHelper => new TranslationDataHelper(TestServer.Host.Services.GetService<ITranslationRepository>());

        protected RelationshipDataHelper RelationshipDataHelper => new RelationshipDataHelper(TestServer.Host.Services.GetService<IRelationshipRepository>());

        protected CategoryDataHelper CategoryDataHelper => new CategoryDataHelper(TestServer.Host.Services.GetService<ICategoryRepository>());

        protected AuthorDataHelper AuthorDataHelper => new AuthorDataHelper(TestServer.Host.Services.GetService<IAuthorRepository>());

        protected BookDataHelper BookDataHelper => new BookDataHelper(TestServer.Host.Services.GetService<IBookRepository>());

        protected HttpResponseMessage Response;

        public HttpClient GetClient()
        {
            _client?.Dispose();
            _client = TestServer.CreateClient();
            return _client;
        }

        public HttpClient GetAdminClient(Guid? userGuid, string userName = "integration.user")
        {
            _authenticatedClient?.Dispose();
            _authenticatedClient = TestServer.CreateClient();
            _authenticatedClient.DefaultRequestHeaders.Add(AuthenticatedTestRequestMiddleware.TestingHeader, AuthenticatedTestRequestMiddleware.TestingHeaderValue);
            _authenticatedClient.DefaultRequestHeaders.Add("my-name", userName);
            _authenticatedClient.DefaultRequestHeaders.Add("my-id", (userGuid ?? Guid.NewGuid()).ToString());
            _authenticatedClient.DefaultRequestHeaders.Add("administrator", bool.TrueString);
            return _authenticatedClient;
        }

        public HttpClient GetContributorClient(Guid? userGuid, string userName = "integration.user")
        {
            _authenticatedClient?.Dispose();
            _authenticatedClient = TestServer.CreateClient();
            _authenticatedClient.DefaultRequestHeaders.Add(AuthenticatedTestRequestMiddleware.TestingHeader, AuthenticatedTestRequestMiddleware.TestingHeaderValue);
            _authenticatedClient.DefaultRequestHeaders.Add("my-name", userName);
            _authenticatedClient.DefaultRequestHeaders.Add("my-id", (userGuid ?? Guid.NewGuid()).ToString());
            _authenticatedClient.DefaultRequestHeaders.Add("contributor", bool.TrueString);
            return _authenticatedClient;
        }

        public HttpClient GetReaderClient(Guid? userGuid, string userName = "integration.user")
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
