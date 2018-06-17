using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Translation
{
    [TestFixture]
    public class WhenAddingTranslationToNonExistingWord : IntegrationTestBase
    {
        private readonly Guid _userId = Guid.NewGuid();
        private TranslationView _translation;
        private Domain.Entities.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                UserId = _userId
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _translation = new TranslationView
            {
                LanguageId = (int)Languages.English,
                Value = "meaning value",
                IsTranspiling = true
            };

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{_dictionary.Id}/words/-34/meanings", _translation);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}