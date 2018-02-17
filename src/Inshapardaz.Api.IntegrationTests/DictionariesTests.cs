using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Inshapardaz.Api.IntegrationTests.DataHelper;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
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
        private DictionaryDataHelpers _dictionaryDataHelper;

        [OneTimeSetUp]
        public void Setup()
        {
            _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id =  -1, IsPublic = true, Name = "Test1"});
            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id =  -2, IsPublic = true, Name = "Test2"});
            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id =  -3, IsPublic = false, Name = "Test3", UserId = Guid.NewGuid()});
            _dictionaryDataHelper.RefreshIndex();

            _response = GetClient().GetAsync("/api/dictionaries").Result;
            _view = JsonConvert.DeserializeObject<DictionariesView>(_response.Content.ReadAsStringAsync().Result);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _dictionaryDataHelper.DeleteDictionary(-1);
            _dictionaryDataHelper.DeleteDictionary(-2);
            _dictionaryDataHelper.DeleteDictionary(-3);
        }

        [Test]
        public void ShouldReturn200()
        {
            _response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldHaveCorrectResponseBody()
        {
            Assert.That(_view, Is.Not.Null);
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

        [Test]
        public void ShouldReturnPublicDictionaries()
        {
            _view.Items.Count().ShouldBe(2);
            _view.Items.ShouldAllBe(d => d.IsPublic);
        }

        [Test]
        public void ShouldNotReturnPrivateDictionaries()
        {
            _view.Items.ShouldNotContain(d => d.IsPublic == false);
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
        private DictionaryDataHelpers _dictionaryDataHelper;

        [OneTimeSetUp]
        public void Setup()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            _dictionaryDataHelper = new DictionaryDataHelpers(Settings);

            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id = -1, IsPublic = true, Name = "Test1", UserId = user1 });
            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id = -2, IsPublic = false, Name = "Test2", UserId = user1 });
            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id = -3, IsPublic = false, Name = "Test3", UserId = user2 });
            _dictionaryDataHelper.CreateDictionary(new Dictionary { Id = -4, IsPublic = true, Name = "Test4", UserId = user2 });
            _dictionaryDataHelper.RefreshIndex();


            _response = GetAuthenticatedClient(user1).GetAsync("/api/dictionaries").Result;
            _view = JsonConvert.DeserializeObject<DictionariesView>(_response.Content.ReadAsStringAsync().Result);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _dictionaryDataHelper.DeleteDictionary(-1);
            _dictionaryDataHelper.DeleteDictionary(-2);
            _dictionaryDataHelper.DeleteDictionary(-3);
            _dictionaryDataHelper.DeleteDictionary(-4);
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

        [Test]
        public void ShouldReturnUsersPublicDictionary()
        {
            _view.Items.ShouldContain(d => d.Id == -1);
        }

        [Test]
        public void ShouldReturnUsersPrivateDictionary()
        {
            _view.Items.ShouldContain(d => d.Id == -2);
        }

        [Test]
        public void ShouldNotReturnOtherUsersPrivateDictionary()
        {
            _view.Items.ShouldNotContain(d => d.Id == -3);
        }

        [Test]
        public void ShouldReturnOtherUsersPublicDictionary()
        {
            _view.Items.ShouldContain(d => d.Id == -4);
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
