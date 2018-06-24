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

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Word
{
    [TestFixture]
    public class WhenGettingWordFromDictionaryAsDifferentUser : IntegrationTestBase
    {
        private WordView _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = true,
                UserId = _userId1,
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

            Response = await GetReaderClient(_userId2).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}");
            _view = JsonConvert.DeserializeObject<WordView>(await Response.Content.ReadAsStringAsync());
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
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Self & l.Href != null);
        }

        [Test]
        public void ShouldReturnDictionaryLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Dictionary & l.Href != null);
        }

        [Test]
        public void ShouldReturnMeaningsLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Meanings & l.Href != null);
        }

        [Test]
        public void ShouldReturnTranslationsLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Translations & l.Href != null);
        }

        [Test]
        public void ShouldReturnRelationshipsLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Relationships & l.Href != null);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnUpdateLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Update);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnDeleteLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Delete & l.Href != null);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnAddMeaningLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.AddMeaning & l.Href != null);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnAddTranslationLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.AddTranslation & l.Href != null);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnAddRelationLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.AddRelation & l.Href != null);
        }

        [Test]
        public void ShouldReturnCorrectWordData()
        {
            _view.Id.ShouldBe(_word.Id);
            _view.Title.ShouldBe(_word.Title);
            _view.TitleWithMovements.ShouldBe(_word.TitleWithMovements);
            _view.Pronunciation.ShouldBe(_word.Pronunciation);
            _view.Description.ShouldBe(_word.Description);
            _view.LanguageId.ShouldBe((int) _word.Language);
            _view.AttributeValue.ShouldBe((int) _word.Attributes);
        }
    }
}