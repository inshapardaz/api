using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Meaning
{
    [TestFixture]
    public class WhenAddingMeaningToWord : IntegrationTestBase
    {
        private MeaningView _view;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private Domain.Entities.Meaning _meaning;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                IsPublic = false,
                UserId = _userId,
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
            };

            DictionaryDataHelper.CreateDictionary(_dictionary);
            WordDataHelper.CreateWord(_dictionary.Id, _word);
            
            _meaning = new Domain.Entities.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };
            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/meanings", _meaning);
            _view = JsonConvert.DeserializeObject<MeaningView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _word.Id);
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
        public void ShouldReturnWordLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Word & l.Href != null);
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
        public void ShouldReturnCorrectMeaningData()
        {
            _view.Context.ShouldBe(_meaning.Context);
            _view.Example.ShouldBe(_meaning.Example);
            _view.Value.ShouldBe(_meaning.Value);
        }
    }
}