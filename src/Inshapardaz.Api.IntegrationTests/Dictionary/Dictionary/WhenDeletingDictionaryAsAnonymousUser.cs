using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Dictionary
{
    [TestFixture]
    public class WhenDeletingDictionaryAsAnonymousUser : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Dictionary.Dictionary _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                Name = "Test1",
                Language = Languages.Avestan,
                UserId = _userId,
                Downloads = new List<DictionaryDownload>
                {
                    new DictionaryDownload { File = "223323", MimeType = MimeTypes.SqlLite},
                    new DictionaryDownload { File = "223324", MimeType = MimeTypes.Csv}
                }
            };

            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _dictionary.Name = "Test1 updated";
            _dictionary.Language = Languages.Arabic;
            Response = await GetClient().DeleteAsync($"/api/dictionaries/{_dictionary.Id}");

            _view = DictionaryDataHelper.GetDictionary(_dictionary.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotDeleteDictionary()
        {
            _view.ShouldNotBeNull();
        }
    }
}