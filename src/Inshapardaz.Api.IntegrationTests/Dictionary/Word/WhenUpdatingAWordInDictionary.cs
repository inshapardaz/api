using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Word
{
    [TestFixture]
    public class WhenUpdatingAWordInDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Word _updatedWord;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };

            _word = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);
            _word  = WordDataHelper.CreateWord(_dictionary.Id, _word);

            _word.Title += "updated";
            _word.TitleWithMovements += "updated";
            _word.Language = Languages.Pali;
            _word.Attributes = GrammaticalType.HarfSoot;
            _word.Description += "updated";
            _word.Pronunciation += "updated";
            
            Response = await GetContributorClient(_userId).PutJson($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}", _word.Map());

            _updatedWord = WordDataHelper.GetWord(_dictionary.Id, _word.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _word.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldUpdateTheWord()
        {
            _updatedWord.Title.ShouldBe(_word.Title);
            _updatedWord.TitleWithMovements.ShouldBe(_word.TitleWithMovements);
            _updatedWord.Pronunciation.ShouldBe(_word.Pronunciation);
            _updatedWord.Description.ShouldBe(_word.Description);
            _updatedWord.Language.ShouldBe(_word.Language);
            _updatedWord.Attributes.ShouldBe(_word.Attributes);
        }
    }
}