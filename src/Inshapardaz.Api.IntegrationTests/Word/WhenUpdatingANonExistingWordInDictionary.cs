using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Word
{
    [TestFixture]
    public class WhenUpdatingANonExistingWordInDictionary : IntegrationTestBase
    {
        private Domain.Entities.Word _updatedWord;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                IsPublic = false,
                UserId = _userId,
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

            DictionaryDataHelper.CreateDictionary(_dictionary);
            DictionaryDataHelper.RefreshIndex();

            _word.Title += "updated";
            _word.TitleWithMovements += "updated";
            _word.Language = Languages.Pali;
            _word.Attributes = GrammaticalType.HarfSoot;
            _word.Description += "updated";
            _word.Pronunciation += "updated";

            Response = await GetContributorClient(_userId).PutJson($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}", _word.Map());

            DictionaryDataHelper.RefreshIndex();
            _updatedWord = WordDataHelper.GetWord(_dictionary.Id, _word.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnBadRequrest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}