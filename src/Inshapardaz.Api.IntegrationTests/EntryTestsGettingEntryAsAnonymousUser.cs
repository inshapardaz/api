using System;
using System.Net;
using System.Net.Http;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests
{
    [TestFixture]
    public class EntryTestsGettingEntryAsAnonymousUser : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private readonly HttpResponseMessage _response;
        private readonly EntryView _view;

        public EntryTestsGettingEntryAsAnonymousUser()
        {
            _server = new TestServer(new WebHostBuilder()
                                         .UseStartup<TestStartup>());
            _client = _server.CreateClient();
            _response = _client.GetAsync("/api").Result;
            _view = JsonConvert.DeserializeObject<EntryView>(_response.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public void ShouldReturn200()
        {
            _response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldHaveCorrectResponseBody()
        {
            _view.ShouldNotBeNull();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
        }

        [Test]
        public void ShouldHaveDictionariesLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Dictionaries && l.Href != null);
        }

        [Test]
        public void ShouldHaveLanguagesLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Languages && l.Href != null);
        }

        [Test]
        public void ShouldHaveAttributesLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Attributes && l.Href != null);
        }

        [Test]
        public void ShouldHaveRelationshipTypesLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.RelationshipTypes && l.Href != null);
        }

        [Test]
        public void ShouldHaveThesaurusLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Thesaurus && l.Href != null);
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
            _response?.Dispose();
        }
    }
}
