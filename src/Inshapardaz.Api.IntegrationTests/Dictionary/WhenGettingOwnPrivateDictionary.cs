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
    public class WhenGettingOwnPrivateDictionary : IntegrationTestBase
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
            DictionaryDataHelper.RefreshIndex();

            Response = await GetContributorClient(userId).GetAsync("/api/dictionaries/-1");
            _view = JsonConvert.DeserializeObject<DictionaryView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(-1);
        }

        [Test]
        public void ShouldReturn200()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
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
        public void ShouldHaveUpdateLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Update && l.Href != null);
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Delete && l.Href != null);
        }

        [Test]
        public void ShouldHaveCreateDownloadLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.CreateDownload && l.Href != null);
        }

        [Test]
        public void ShouldHaveIndexLinks()
        {
            _view.Indexes.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldHaveDownloadLinks()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Download && l.Href != null && l.Type == MimeTypes.SqlLite);
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Download && l.Href != null && l.Type == MimeTypes.Csv);
        }

        [Test]
        public void ShouldReturnCorrectDictionaryMetadata()
        {
            _view.Id.ShouldBe(_dictionary.Id);
            _view.Name.ShouldBe(_dictionary.Name);
            _view.Language.ShouldBe(_dictionary.Language);
            _view.IsPublic.ShouldBe(_dictionary.IsPublic);
            _view.UserId.ShouldBe(_dictionary.UserId);
            _view.WordCount.ShouldBe(0);
            _view.Indexes.ShouldNotBeEmpty();
        }
    }
}