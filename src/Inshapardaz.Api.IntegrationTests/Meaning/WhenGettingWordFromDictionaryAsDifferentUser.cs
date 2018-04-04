using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Meaning
{
    [TestFixture, Ignore("Security not implemented completely")]
    public class WhenGettingWordFromDictionaryAsDifferentUser : IntegrationTestBase
    {
        private WordView _view;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                IsPublic = true,
                UserId = _userId1,
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
                DictionaryId = _dictionary.Id
            };

            DictionaryDataHelper.CreateDictionary(_dictionary);
            WordDataHelper.CreateWord(_dictionary.Id, _word);
            DictionaryDataHelper.RefreshIndex();

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

        [Test]
        public void ShouldNotReturnUpdateLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Update);
        }

        [Test]
        public void ShouldNotReturnDeleteLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Delete & l.Href != null);
        }

        [Test]
        public void ShouldReturnnotAddMeaningLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.AddMeaning & l.Href != null);
        }

        [Test]
        public void ShouldNotReturnAddTranslationLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.AddTranslation & l.Href != null);
        }

        [Test]
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