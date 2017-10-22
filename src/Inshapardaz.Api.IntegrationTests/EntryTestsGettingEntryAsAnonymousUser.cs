using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
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
                                         .UseStartup<TestStartup>());
            _client = _server.CreateClient();
            _response = _client.GetAsync("/").Result;
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
