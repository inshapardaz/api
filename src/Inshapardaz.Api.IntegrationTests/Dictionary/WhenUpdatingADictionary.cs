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
    public partial class DictionariesTests
    {
        [TestFixture]
        public class WhenUpdatingADictionary : IntegrationTestBase
        {
            private Domain.Entities.Dictionary _createdDictionary;
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
                Response = await GetAuthenticatedClient(_userId).PutJson($"/api/dictionaries/{_dictionary.Id}", _dictionary);

                DictionaryDataHelper.RefreshIndex();
                _createdDictionary = DictionaryDataHelper.GetDictionary(_dictionary.Id);
            }

            [OneTimeTearDown]
            public void Cleanup()
            {
                DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            }

            [Test]
            public void ShouldReturnCreated()
            {
                Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            }
            
            [Test]
            public void ShouldCreateCorrectDictionary()
            {
                _createdDictionary.Name.ShouldBe(_dictionary.Name);
                _createdDictionary.Language.ShouldBe(_dictionary.Language);
                _createdDictionary.IsPublic.ShouldBe(_dictionary.IsPublic);
                _createdDictionary.UserId.ShouldBe(_userId);
            }
        }
    }
}
