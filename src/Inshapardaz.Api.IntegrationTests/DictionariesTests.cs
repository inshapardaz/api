using System;
using System.Net;
using System.Net.Http;
using FizzWare.NBuilder;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Data.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests
{
    [TestFixture]
    public class DictionariesTests : IntegrationTestBase
    {
        private readonly HttpResponseMessage _response;
        private readonly DictionariesView _view;

        public DictionariesTests()
        {
            var dictionaries = Builder<Dictionary>.CreateListOfSize(2).Build();
            //DatabaseContext.Dictionary.AddRange(dictionaries);

            _response = GetClient().GetAsync("/api/dictionaries").Result;
            _view = JsonConvert.DeserializeObject<DictionariesView>(_response.Content.ReadAsStringAsync().Result);
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
        public void ShouldNotHaveCreateLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Create && l.Href != null);
        }
        
        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            if (isDisposing)
            {
                _response?.Dispose();
            }
        }
    }
}
