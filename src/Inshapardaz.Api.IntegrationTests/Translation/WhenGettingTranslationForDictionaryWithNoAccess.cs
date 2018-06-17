using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Translation
{
    [TestFixture]
    public class WhenGettingTranslationForDictionaryWithNoAccess : IntegrationTestBase
    {
        private TranslationView _view;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Translation _translation;

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

            _translation = TranslationDataHelper.CreateTranslation(_dictionary.Id, _word.Id, new Domain.Entities.Translation { Language = Languages.Arabic, Value = "sdsd1", IsTrasnpiling = true });

            Response = await GetContributorClient(Guid.NewGuid()).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/translations/{_translation.Id}");
            _view = JsonConvert.DeserializeObject<TranslationView>(await Response.Content.ReadAsStringAsync());
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