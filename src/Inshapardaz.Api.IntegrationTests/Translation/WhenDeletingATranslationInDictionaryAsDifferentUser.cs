using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Translation
{
    [TestFixture]
    public class WhenDeletingATranslationInDictionaryAsDifferentUser : IntegrationTestBase
    {
        private Domain.Entities.Translation _translation;
        private Domain.Entities.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _translationId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = true,
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

            var translation = new Domain.Entities.Translation
            {
                IsTrasnpiling = true,
                Value = "translation value",
                Language = Languages.Bangali
            };

            translation = TranslationDataHelper.CreateTranslation(_dictionary.Id, _wordId, translation);

            _translationId = translation.Id;

            Response = await GetContributorClient(Guid.NewGuid()).DeleteAsync($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/translations/{_translationId}");

            _translation = TranslationDataHelper.GetTranslation(_dictionary.Id, word.Id, _translationId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TranslationDataHelper.DeleteTranslation(_dictionary.Id, _wordId, _translation.Id);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldnotHaveDeletedTheWord()
        {
            _translation.ShouldNotBeNull();
        }
    }
}