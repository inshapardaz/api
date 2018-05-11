using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Word
{
    [TestFixture]
    public class WhenAddingAWordToOtherUsersPrivateDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary _dictionary;
        private WordView _word;
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                IsPublic = false,
                UserId = _userId1,
                Name = "Test1"
            };

            _word = new WordView
            {
                Id = -2,
                Title = "abc",
                TitleWithMovements = "xyz",
                LanguageId = (int)Languages.Bangali,
                Pronunciation = "pas",
                AttributeValue = (int)GrammaticalType.FealImdadi & (int)GrammaticalType.Male,
            };

            DictionaryDataHelper.CreateDictionary(_dictionary);

            Response = await GetContributorClient(_userId2).PostJson($"/api/dictionaries/{_dictionary.Id}/words", _word);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}