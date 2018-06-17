using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Meaning
{
    [TestFixture]
    public class WhenUpdatingAMeaningInPrivateDictionaryOfAnotherUser: IntegrationTestBase
    {
        private Domain.Entities.Meaning _translation;
        private Domain.Entities.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _meaningId;
        private Domain.Entities.Meaning _oldMeaning;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            var word = new Domain.Entities.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            word = WordDataHelper.CreateWord(_dictionary.Id, word);
            _wordId = word.Id;

            _oldMeaning = new Domain.Entities.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            _oldMeaning = MeaningDataHelper.CreateMeaning(_dictionary.Id, _wordId, _oldMeaning);

            _meaningId = _oldMeaning.Id;

            _oldMeaning.Value = "updated value";

            Response = await GetContributorClient(Guid.NewGuid()).PutJson($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/meanings/{_meaningId}", _oldMeaning.Map());

            _translation = MeaningDataHelper.GetMeaning(_dictionary.Id, word.Id, _meaningId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            MeaningDataHelper.DeleteMeaning(_dictionary.Id, _wordId, _meaningId);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotHaveUpdateTheMeaning()
        {
            _translation.Context.ShouldBe(_oldMeaning.Context);
            _translation.Value.ShouldNotBe(_oldMeaning.Value);
            _translation.Example.ShouldBe(_oldMeaning.Example);
        }
    }
}