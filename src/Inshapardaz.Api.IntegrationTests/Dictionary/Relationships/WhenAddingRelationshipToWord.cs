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

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Relationships
{
    [TestFixture]
    public class WhenAddingRelationshipToWord : IntegrationTestBase
    {
        private RelationshipView _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _sourceWord;
        private Domain.Entities.Dictionary.Word _relatedWord;
        private RelationshipView _relationshipView;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _sourceWord = new Domain.Entities.Dictionary.Word
            {
                DictionaryId = _dictionary.Id,
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            _relatedWord = new Domain.Entities.Dictionary.Word
            {
                DictionaryId = _dictionary.Id,
                Title = "aBdvsrr",
                TitleWithMovements = "xyz",
                Language = Languages.German,
                Pronunciation = "pas",
                Attributes = GrammaticalType.Feal & GrammaticalType.Male,
            };

            _sourceWord = WordDataHelper.CreateWord(_dictionary.Id, _sourceWord);
            _relatedWord = WordDataHelper.CreateWord(_dictionary.Id, _relatedWord);
            
            _relationshipView = new RelationshipView
            {
                SourceWordId = _sourceWord.Id,
                RelatedWordId = _relatedWord.Id,
                RelationTypeId = 4
            };

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{_dictionary.Id}/words/{_sourceWord.Id}/relationships", _relationshipView);
            _view = JsonConvert.DeserializeObject<RelationshipView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _sourceWord.Id);
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
            _view.RelatedWordId.ShouldBe(_relationshipView.RelatedWordId);
            _view.RelatedWord.ShouldBe(_relatedWord.Title);
            _view.SourceWordId.ShouldBe(_relationshipView.SourceWordId);
            _view.SourceWord.ShouldBe(_sourceWord.Title);
            _view.RelationTypeId.ShouldBe(_relationshipView.RelationTypeId);
        }
    }
}