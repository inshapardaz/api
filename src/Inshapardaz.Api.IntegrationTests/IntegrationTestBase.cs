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
        private readonly TestServer _testServer;
        private HttpClient _authenticatedClient;
        private HttpClient _client;
        protected IDatabaseContext DatabaseContext;

        protected IntegrationTestBase()
        {
            _testServer = new TestServer(new WebHostBuilder()
                                             .UseStartup<TestStartup>());
            DatabaseContext = _testServer.Host.Services.GetService<IDatabaseContext>();
        }

        protected HttpClient GetClient()
        {
            _client?.Dispose();
            _client = _testServer.CreateClient();
            return _client;
        }

        protected HttpClient GetAuthenticatedClient(Guid? userGuid)
        {
            _authenticatedClient?.Dispose();
            _authenticatedClient = _testServer.CreateClient();
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
                _testServer?.Dispose();
                _authenticatedClient?.Dispose();
                _client?.Dispose();
            }
        }
    }
}
