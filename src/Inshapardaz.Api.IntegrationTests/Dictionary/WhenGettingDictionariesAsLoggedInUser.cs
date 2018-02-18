using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    public partial class DictionariesTests
    {
        [TestFixture]
        public class WhenGettingDictionariesAsLoggedInUser : IntegrationTestBase
        {
            private DictionariesView _view;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var user1 = Guid.NewGuid();
                var user2 = Guid.NewGuid();

                DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary {Id = -1, IsPublic = true, Name = "Test1", UserId = user1});
                DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary {Id = -2, IsPublic = false, Name = "Test2", UserId = user1});
                DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary {Id = -3, IsPublic = false, Name = "Test3", UserId = user2});
                DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary {Id = -4, IsPublic = true, Name = "Test4", UserId = user2});
                DictionaryDataHelper.RefreshIndex();


                Response = await GetAuthenticatedClient(user1).GetAsync("/api/dictionaries");
                _view = JsonConvert.DeserializeObject<DictionariesView>(await Response.Content.ReadAsStringAsync());
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                DictionaryDataHelper.DeleteDictionary(-1);
                DictionaryDataHelper.DeleteDictionary(-2);
                DictionaryDataHelper.DeleteDictionary(-3);
                DictionaryDataHelper.DeleteDictionary(-4);
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
        }
    }
}
