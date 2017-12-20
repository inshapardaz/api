using System;
using System.Net.Http;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
            TestServer = new TestServer(new WebHostBuilder()
                                             .UseStartup<TestStartup>());
        }

        protected IDatabaseContext DatabaseContext => TestServer.Host.Services.GetService<IDatabaseContext>();

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
                _authenticatedClient?.Dispose();
                _client?.Dispose();
            }
        }
    }
}
