using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    public partial class DictionariesTests
    {
        [TestFixture]
        public class WhenDeletingADictionary : IntegrationTestBase
        {
            private Domain.Entities.Dictionary _existedDictionary;
            private Domain.Entities.Dictionary _dictionary;
            private readonly Guid _userId = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionary = new Domain.Entities.Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload { Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite },
                        new DictionaryDownload { Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv }
                    }
                };

                DictionaryDataHelper.CreateDictionary(_dictionary);
                DictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                Response = await GetAuthenticatedClient(_userId).DeleteAsync($"/api/dictionaries/{_dictionary.Id}");

                DictionaryDataHelper.RefreshIndex();
                _existedDictionary = DictionaryDataHelper.GetDictionary(_dictionary.Id);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnNoContent()
            {
                Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            }

            [Test]
            public void ShouldRemoveDictionary()
            {
                _existedDictionary.ShouldBeNull();
            }
        }
    }
}
