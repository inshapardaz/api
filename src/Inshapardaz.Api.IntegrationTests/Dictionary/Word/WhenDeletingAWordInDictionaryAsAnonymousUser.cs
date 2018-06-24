using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Word
{
    [TestFixture]
    public class WhenDeletingAWordInDictionaryAsAnonymousUser : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Word _word;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };

            var word = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);
            word = WordDataHelper.CreateWord(_dictionary.Id, word);

            _wordId = word.Id;
            word.Title += "updated";
            word.TitleWithMovements += "updated";
            word.Language = Languages.Pali;
            word.Attributes = GrammaticalType.HarfSoot;
            word.Description += "updated";
            word.Pronunciation += "updated";

            Response = await GetClient().DeleteAsync($"/api/dictionaries/{_dictionary.Id}/words/{word.Id}");

            _word = WordDataHelper.GetWord(_dictionary.Id, word.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotHaveDeletedTheWord()
        {
            _word.ShouldNotBeNull();
        }
    }
}