using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Inshapardaz.Api.IntegrationTests
{
    public class EntryTestsGettingEntryAsAnonymousUser : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly HttpResponseMessage _response;

        public EntryTestsGettingEntryAsAnonymousUser()
        {
            _server = new TestServer(new WebHostBuilder()
                                         .UseStartup<Startup>());
            _client = _server.CreateClient();
            _response = _client.GetAsync("/api").Result;
        }

        [Fact]
        public void ShouldReturn200()
        {
            _response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
            _response?.Dispose();
        }
    }
}
