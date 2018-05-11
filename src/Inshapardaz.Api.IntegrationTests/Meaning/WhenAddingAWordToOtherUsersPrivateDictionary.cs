using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Meaning
{
    [TestFixture]
    public class WhenAddingMeaningToOtherUsersPrivateDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();
        private Domain.Entities.Meaning _meaning;

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

            _word = new Domain.Entities.Word
            {
                Id = -2,
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };
            _meaning = new Domain.Entities.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            DictionaryDataHelper.CreateDictionary(_dictionary);
            WordDataHelper.CreateWord(_dictionary.Id, _word);

            Response = await GetContributorClient(_userId2).PostJson($"/api/dictionaries/{_dictionary.Id}/words/-34/meanings", _meaning);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _word.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}