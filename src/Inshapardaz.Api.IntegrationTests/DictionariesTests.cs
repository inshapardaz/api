using System;
using System.Net;
using System.Net.Http;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests
{
    [TestFixture]
    public class WhenGettingDictionariesAnonymously : IntegrationTestBase
    {
        private HttpResponseMessage _response;
        private DictionariesView _view;

        [OneTimeSetUp]
        public void Setup()
        {
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

    [TestFixture]
    public class WhenGettingDictionariesAsLoggedInUser : IntegrationTestBase
    {
        private HttpResponseMessage _response;
        private DictionariesView _view;

        [OneTimeSetUp]
        public void Setup()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            DatabaseContext.Dictionary.Add(new Dictionary { Id = 1, IsPublic = false, UserId = user1 });
            DatabaseContext.Dictionary.Add(new Dictionary { Id = 2, IsPublic = true, UserId = user2 });
            DatabaseContext.Dictionary.Add(new Dictionary { Id = 3, IsPublic = false, UserId = user2 });
            DatabaseContext.Dictionary.Add(new Dictionary { Id = 4, IsPublic = true, UserId = user1 });
            DatabaseContext.SaveChanges();

            _response = GetAuthenticatedClient(user1).GetAsync("/api/dictionaries").Result;
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
        public void ShouldHaveCreateLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Create && l.Href != null);
        }

        //[Test]
        //public void ShouldReturnUsersPublicDictionary()
        //{
        //    _view.Items.ShouldContain(d => d.Id == 4);
        //}

        //[Test]
        //public void ShouldReturnUsersPrivateDictionary()
        //{
        //    _view.Items.ShouldContain(d => d.Id == 1);
        //}

        [Test]
        public void ShouldNotReturnOtherUsersPrivateDictionary()
        {
            _view.Items.ShouldNotContain(d => d.Id == 3);
        }

        //[Test]
        //public void ShouldReturnOtherUsersPublicDictionary()
        //{
        //    _view.Items.ShouldContain(d => d.Id == 2);
        //}

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
