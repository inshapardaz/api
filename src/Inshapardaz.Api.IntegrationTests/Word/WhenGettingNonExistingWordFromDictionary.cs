using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Word
{
    [TestFixture]
    public class WhenGettingNonExistingWordFromDictionary : IntegrationTestBase
    {
        private WordView _view;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = true,
                UserId = Guid.NewGuid(),
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

            Response = await GetClient().GetAsync($"/api/dictionaries/{_dictionary.Id}/words/-999999");
            _view = JsonConvert.DeserializeObject<WordView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Test]
        public void ShouldReturnNoWordData()
        {
            _view.ShouldBeNull();
        }
    }
}