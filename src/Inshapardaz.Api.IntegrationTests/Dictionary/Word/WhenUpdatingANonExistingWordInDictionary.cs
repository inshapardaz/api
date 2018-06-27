using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
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
    public class WhenUpdatingANonExistingWordInDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Word _updatedWord;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private WordView _view;

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

            _word.Title += "updated";
            _word.TitleWithMovements += "updated";
            _word.Language = Languages.Pali;
            _word.Attributes = GrammaticalType.HarfSoot;
            _word.Description += "updated";
            _word.Pronunciation += "updated";

            Response = await GetContributorClient(_userId).PutJson($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}", _word.Map());

            _updatedWord = WordDataHelper.GetWord(_dictionary.Id, _word.Id);

            _view = JsonConvert.DeserializeObject<WordView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnCreated()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Test]
        public void ShouldReturnLocationHeader()
        {
            Response.Headers.Location.ShouldNotBeNull();
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
        public void ShouldReturnUpdateLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Update & l.Href != null);
        }

        [Test]
        public void ShouldReturnDeleteLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Delete & l.Href != null);
        }

        [Test]
        public void ShouldReturnAddMeaningLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.AddMeaning & l.Href != null);
        }

        [Test]
        public void ShouldReturnAddTranslationLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.AddTranslation & l.Href != null);
        }

        [Test]
        public void ShouldReturnAddRelationLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.AddRelation & l.Href != null);
        }

        [Test]
        public void ShouldReturnCorrectWordData()
        {
            _view.Title.ShouldBe(_word.Title);
            _view.TitleWithMovements.ShouldBe(_word.TitleWithMovements);
            _view.Pronunciation.ShouldBe(_word.Pronunciation);
            _view.Description.ShouldBe(_word.Description);
            _view.LanguageId.ShouldBe((int)_word.Language);
            _view.AttributeValue.ShouldBe((int)_word.Attributes);
        }
    }
}