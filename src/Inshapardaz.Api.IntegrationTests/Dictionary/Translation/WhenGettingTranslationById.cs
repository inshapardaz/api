using System;
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
    public class WhenGettingTranslationById : IntegrationTestBase
    {
        private TranslationView _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Dictionary.Translation _translation;

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

            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);
            _word = WordDataHelper.CreateWord(_dictionary.Id, _word);

            _translation = TranslationDataHelper.CreateTranslation(_dictionary.Id, _word.Id, new Domain.Entities.Dictionary.Translation { Language = Languages.Arabic, Value = "sdsd1", IsTrasnpiling= true });

            Response = await GetContributorClient(_userId).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/translations/{_translation.Id}");
            _view = JsonConvert.DeserializeObject<TranslationView>(await Response.Content.ReadAsStringAsync());
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
        public void ShouldReturnTranslationWithCorrectId()
        {
            _view.Id.ShouldBe(_translation.Id);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectTranspiling()
        {
            _view.IsTranspiling.ShouldBe(_translation.IsTrasnpiling);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectLanguage()
        {
            _view.LanguageId.ShouldBe((int)_translation.Language);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectValue()
        {
            _view.Value.ShouldBe(_translation.Value);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectWordId()
        {
            _view.WordId.ShouldBe(_translation.WordId);
        }

        [Test]
        public void ShouldReturnTranslationWithLanguageName()
        {
            _view.WordId.ShouldNotBeNull();
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldReturnUpdateLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldReturnDeleteLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Delete);
        }

        [Test]
        public void ShouldReturnWordLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Word);
        }
    }
}