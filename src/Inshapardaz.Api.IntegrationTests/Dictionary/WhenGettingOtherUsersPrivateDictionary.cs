using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    [TestFixture]
    public class WhenGettingOtherUsersPrivateDictionary : IntegrationTestBase
    {
        private DictionaryView _view;
        private Domain.Entities.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var userId = Guid.NewGuid();
            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                IsPublic = false,
                Name = "Test1",
                UserId = userId,
                Downloads = new List<DictionaryDownload>
                {
                    new DictionaryDownload {Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite},
                    new DictionaryDownload {Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv}
                }
            };
            DictionaryDataHelper.CreateDictionary(_dictionary);

            Response = await GetContributorClient(Guid.NewGuid()).GetAsync("/api/dictionaries/-1");
            _view = JsonConvert.DeserializeObject<DictionaryView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(-1);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldHaveEmptyResponseBody()
        {
            Assert.That(_view, Is.Null);
        }
    }
}