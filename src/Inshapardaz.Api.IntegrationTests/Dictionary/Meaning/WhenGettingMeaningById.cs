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

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Meaning
{
    [TestFixture]
    public class WhenGettingMeaningById : IntegrationTestBase
    {
        private MeaningView _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Dictionary.Meaning _meaning1;

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

            _meaning1 = MeaningDataHelper.CreateMeaning(_dictionary.Id, _word.Id, new Domain.Entities.Dictionary.Meaning { Context = "ctx1", Value = "sdsd1", Example = "None" });

            Response = await GetContributorClient(_userId).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/meanings/{_meaning1.Id}");
            _view = JsonConvert.DeserializeObject<MeaningView>(await Response.Content.ReadAsStringAsync());
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
        public void ShouldReturnMeaningWithCorrectId()
        {
            _view.Id.ShouldBe(_meaning1.Id);
        }

        [Test]
        public void ShouldReturnMeaningWithCorrectContext()
        {
            _view.Context.ShouldBe(_meaning1.Context);
        }

        [Test]
        public void ShouldReturnMeaningWithCorrectExample()
        {
            _view.Example.ShouldBe(_meaning1.Example);
        }

        [Test]
        public void ShouldReturnMeaningWithCorrectValue()
        {
            _view.Value.ShouldBe(_meaning1.Value);
        }

        [Test]
        public void ShouldReturnMeaningWithCorrectWordId()
        {
            _view.WordId.ShouldBe(_meaning1.WordId);
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