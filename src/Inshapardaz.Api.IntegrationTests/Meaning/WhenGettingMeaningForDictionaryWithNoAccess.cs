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
    [TestFixture]
    public class WhenGettingMeaningForDictionaryWithNoAccess : IntegrationTestBase
    {
        private MeaningView _view;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Meaning _meaning1;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };

            _word = new Domain.Entities.Word
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

            _meaning1 = MeaningDataHelper.CreateMeaning(_dictionary.Id, _word.Id, new Domain.Entities.Meaning { Context = "ctx1", Value = "sdsd1", Example = "None" });

            Response = await GetContributorClient(Guid.NewGuid()).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/meanings/{_meaning1.Id}");
            _view = JsonConvert.DeserializeObject<MeaningView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotContainAnyData()
        {
            _view.ShouldBeNull();
        }
    }
}