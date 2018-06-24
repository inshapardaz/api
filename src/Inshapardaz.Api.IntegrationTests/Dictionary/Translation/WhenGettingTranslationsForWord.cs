using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Translation
{
    [TestFixture]
    public class WhenGettingTranslationsForWord : IntegrationTestBase
    {
        private IEnumerable<TranslationView> _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Dictionary.Word _word2;
        private Domain.Entities.Dictionary.Translation _translation1;
        private Domain.Entities.Dictionary.Translation _translation2;
        private Domain.Entities.Dictionary.Translation _translation3;

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
                DictionaryId = _dictionary.Id
            };

            _word2 = new Domain.Entities.Dictionary.Word
            {
                Title = "AbcDeFg",
                TitleWithMovements = "xyz",
                Language = Languages.Avestan,
                Pronunciation = "uwweui",
                Attributes = GrammaticalType.HarfNafi & GrammaticalType.Female,
                DictionaryId = _dictionary.Id
            };


            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);
            _word = WordDataHelper.CreateWord(_dictionary.Id, _word);
            _word2 = WordDataHelper.CreateWord(_dictionary.Id, _word2);

            _translation1 = TranslationDataHelper.CreateTranslation(_dictionary.Id, _word.Id, new Domain.Entities.Dictionary.Translation { Language = Languages.Arabic, Value = "sdsd1", IsTrasnpiling = true });
            _translation2 = TranslationDataHelper.CreateTranslation(_dictionary.Id, _word.Id, new Domain.Entities.Dictionary.Translation { Language = Languages.Avestan, Value = "sdsd2", IsTrasnpiling = true });
            _translation3 = TranslationDataHelper.CreateTranslation(_dictionary.Id, _word2.Id, new Domain.Entities.Dictionary.Translation { Language = Languages.Arabic, Value = "sdsd3", IsTrasnpiling = true });

            Response = await GetContributorClient(_userId).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/translations");
            _view = JsonConvert.DeserializeObject<IEnumerable<TranslationView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnTranslations()
        {
            _view.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldContainTranslationForTheWord()
        {
            _view.ShouldContain(m => m.Id == _translation1.Id);
            _view.ShouldContain(m => m.Id == _translation2.Id);
        }

        [Test]
        public void ShouldNotContainTranslationForOtherWord()
        {
            _view.ShouldNotContain(m => m.Id == _translation3.Id);
        }

    }
}