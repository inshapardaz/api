using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Meaning
{
    [TestFixture]
    public class WhenUpdatingMeaningInDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Meaning _meaning;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _meaningId;
        private Domain.Entities.Dictionary.Meaning _oldMeaning;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            var word = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            word = WordDataHelper.CreateWord(_dictionary.Id, word);
            _wordId = word.Id;

            _oldMeaning = new Domain.Entities.Dictionary.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            _oldMeaning = MeaningDataHelper.CreateMeaning(_dictionary.Id, _wordId, _oldMeaning);

            _meaningId = _oldMeaning.Id;

            _oldMeaning.Value = "New Meaning";

            Response = await GetContributorClient(_userId).PutJson($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/meanings/{_meaningId}", _oldMeaning.Map());

            _meaning = MeaningDataHelper.GetMeaning(_dictionary.Id, word.Id, _meaningId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            MeaningDataHelper.DeleteMeaning(_dictionary.Id, _wordId, _meaningId);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveUpdateTheMeaning()
        {
            _meaning.Value.ShouldBe(_oldMeaning.Value);
            _meaning.Context.ShouldBe(_oldMeaning.Context);
            _meaning.Example.ShouldBe(_oldMeaning.Example);
        }
    }
}