using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    [TestFixture]
    public class WhenCreatingADictionaryAsAnonymousUser : IntegrationTestBase
    {
        private Domain.Entities.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var userId = Guid.NewGuid();
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = false,
                Name = "Test1",
                UserId = userId,
                Downloads = new List<DictionaryDownload>
                {
                    new DictionaryDownload { File = "223323", MimeType = MimeTypes.SqlLite },
                    new DictionaryDownload { File = "223324", MimeType = MimeTypes.Csv }
                }
            };

            Response = await GetClient().PostJson("/api/dictionaries", _dictionary);
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
    }
}