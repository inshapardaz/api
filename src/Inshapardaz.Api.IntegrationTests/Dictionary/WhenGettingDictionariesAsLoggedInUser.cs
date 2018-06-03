using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    [TestFixture]
    public class WhenGettingDictionariesAsLoggedInUser : IntegrationTestBase
    {
        private DictionariesView _view;
        private Domain.Entities.Dictionary _dictionary1;
        private Domain.Entities.Dictionary _dictionary2;
        private Domain.Entities.Dictionary _dictionary3;
        private Domain.Entities.Dictionary _dictionary4;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            _dictionary1 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = true, Name = "Test1", UserId = user1 });
            _dictionary2 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = false, Name = "Test2", UserId = user1 });
            _dictionary3 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = false, Name = "Test3", UserId = user2 });
            _dictionary4 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = true, Name = "Test4", UserId = user2 });


            Response = await GetContributorClient(user1).GetAsync("/api/dictionaries");
            _view = JsonConvert.DeserializeObject<DictionariesView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary1.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary2.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary3.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary4.Id);
        }

        [Test]
        public void ShouldReturn200()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
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
            _view.Items.ShouldContain(d => d.Id == _dictionary1.Id);
        }

        [Test]
        public void ShouldReturnUsersPrivateDictionary()
        {
            _view.Items.ShouldContain(d => d.Id == _dictionary2.Id);
        }

        [Test]
        public void ShouldNotReturnOtherUsersPrivateDictionary()
        {
            _view.Items.ShouldNotContain(d => d.Id == _dictionary3.Id);
        }

        [Test]
        public void ShouldReturnOtherUsersPublicDictionary()
        {
            _view.Items.ShouldContain(d => d.Id == _dictionary4.Id);
        }
    }
}