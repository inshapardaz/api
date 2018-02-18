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
        public class WhenDeletingOtherUsersDictionary : IntegrationTestBase
        {
            private Domain.Entities.Dictionary _dictionary;
            private readonly Guid _userId1 = Guid.NewGuid();
            private readonly Guid _userId2 = Guid.NewGuid();

            [OneTimeSetUp]
            public async Task Setup()
            {
                _dictionary = new Domain.Entities.Dictionary
                {
                    Id = -1,
                    IsPublic = false,
                    Name = "Test1",
                    Language = Languages.Avestan,
                    UserId = _userId1,
                    Downloads = new List<DictionaryDownload>
                    {
                        new DictionaryDownload {Id = -101, DictionaryId = -1, File = "223323", MimeType = MimeTypes.SqlLite},
                        new DictionaryDownload {Id = -102, DictionaryId = -1, File = "223324", MimeType = MimeTypes.Csv}
                    }
                };

                DictionaryDataHelper.CreateDictionary(_dictionary);
                DictionaryDataHelper.RefreshIndex();

                _dictionary.Name = "Test1 updated";
                _dictionary.Language = Languages.Arabic;
                Response = await GetAuthenticatedClient(_userId2).DeleteAsync($"/api/dictionaries/{_dictionary.Id}");
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
        }
    }
}
