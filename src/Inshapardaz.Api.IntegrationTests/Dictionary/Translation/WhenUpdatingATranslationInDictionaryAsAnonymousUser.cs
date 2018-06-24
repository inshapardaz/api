using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Translation
{
    [TestFixture]
    public class WhenUpdatingATranslationInDictionaryAsAnonymousUser : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Translation _translation;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _translationId;
        private Domain.Entities.Dictionary.Translation _oldTranslation;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = true,
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

            _oldTranslation = new Domain.Entities.Dictionary.Translation
            {
                IsTrasnpiling = true,
                Value = "translation value",
                Language = Languages.English
            };

            _oldTranslation = TranslationDataHelper.CreateTranslation(_dictionary.Id, _wordId, _oldTranslation);

            _translationId = _oldTranslation.Id;

            _oldTranslation.Language = Languages.German;

            Response = await GetContributorClient(Guid.Empty).PutJson($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/translations/{_translationId}", _oldTranslation.Map());

            _translation = TranslationDataHelper.GetTranslation(_dictionary.Id, word.Id, _translationId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TranslationDataHelper.DeleteTranslation(_dictionary.Id, _wordId, _translationId);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotHaveUpdateTheTranslation()
        {
            _translation.Language.ShouldNotBe(_oldTranslation.Language);
            _translation.Value.ShouldBe(_oldTranslation.Value);
            _translation.IsTrasnpiling.ShouldBe(_oldTranslation.IsTrasnpiling);
        }
    }
}