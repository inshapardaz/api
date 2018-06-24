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

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Meaning
{
    [TestFixture]
    public class WhenGettingMeaningsForWord : IntegrationTestBase
    {
        private IEnumerable<MeaningView> _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Dictionary.Word _word2;
        private Domain.Entities.Dictionary.Meaning _meaning1;
        private Domain.Entities.Dictionary.Meaning _meaning2;
        private Domain.Entities.Dictionary.Meaning _meaning3;

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

            _meaning1 = MeaningDataHelper.CreateMeaning(_dictionary.Id, _word.Id, new Domain.Entities.Dictionary.Meaning { Context = "ctx", Value = "sdsd1", Example = "None" });
            _meaning2 = MeaningDataHelper.CreateMeaning(_dictionary.Id, _word.Id, new Domain.Entities.Dictionary.Meaning { Context = "ctx", Value = "sdsd2", Example = "None" });
            _meaning3 = MeaningDataHelper.CreateMeaning(_dictionary.Id, _word2.Id, new Domain.Entities.Dictionary.Meaning { Context = "ctx", Value = "erer2", Example = "None" });

            Response = await GetContributorClient(_userId).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/meanings");
            _view = JsonConvert.DeserializeObject<IEnumerable<MeaningView>>(await Response.Content.ReadAsStringAsync());
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
        public void ShouldReturnMeanings()
        {
            _view.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldContainMeaningForTheWord()
        {
            _view.ShouldContain(m => m.Id == _meaning1.Id);
            _view.ShouldContain(m => m.Id == _meaning2.Id);
        }

        [Test]
        public void ShouldNotContainMeaningForOtherWord()
        {
            _view.ShouldNotContain(m => m.Id == _meaning3.Id);
        }

    }
}