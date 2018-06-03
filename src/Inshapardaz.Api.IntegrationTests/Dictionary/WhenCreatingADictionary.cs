using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    [TestFixture]
    public class WhenCreatingADictionary : IntegrationTestBase
    {
        private DictionaryView _view;
        private Domain.Entities.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = false,
                Name = "Test1",
                Downloads = new List<DictionaryDownload>
                {
                    new DictionaryDownload { File = "223323", MimeType = MimeTypes.SqlLite },
                    new DictionaryDownload { File = "223324", MimeType = MimeTypes.Csv }
                }
            };

            Response = await GetContributorClient(_userId).PostJson("/api/dictionaries", _dictionary);
            _view = JsonConvert.DeserializeObject<DictionaryView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_view.Id);
        }

        [Test]
        public void ShouldReturnCreated()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Test]
        public void ShouldReturnNewItemLink()
        {
            Response.Headers.Location.ShouldNotBeNull();
        }

        [Test]
        public void ShouldReturnCreatedDictionary()
        {
            _view.ShouldNotBeNull();
        }

        [Test]
        public void ShouldCreateCorrectDictionary()
        {
            _view.Name.ShouldBe(_dictionary.Name);
            _view.Language.ShouldBe(_dictionary.Language);
            _view.IsPublic.ShouldBe(_dictionary.IsPublic);
            _view.UserId.ShouldBe(_userId);
            _view.WordCount.ShouldBe(0);
            _view.Indexes.ShouldNotBeEmpty();
        }
    }
}