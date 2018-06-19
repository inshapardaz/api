using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Relationships
{
    [TestFixture]
    public class WhenGettingRelationshipForDictionaryWithNoAccess : IntegrationTestBase
    {
        private RelationshipView _view;
        private Domain.Entities.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private WordRelation _relationship;
        private Domain.Entities.Word _sourceWord;
        private Domain.Entities.Word _relatedWord;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _sourceWord = new Domain.Entities.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male
            };

            _sourceWord = WordDataHelper.CreateWord(_dictionary.Id, _sourceWord);

            _relatedWord = new Domain.Entities.Word
            {
                Title = "abc2",
                TitleWithMovements = "xyz2",
                Language = Languages.Arabic,
                Pronunciation = "fhjkshf",
                Attributes = GrammaticalType.SiftIshara
            };
            _relatedWord = WordDataHelper.CreateWord(_dictionary.Id, _relatedWord);


            _relationship = new WordRelation
            {
                SourceWordId = _sourceWord.Id,
                RelatedWordId = _relatedWord.Id,
                RelationType = RelationType.Compound
            };

            _relationship = RelationshipDataHelper.CreateRelationship(_dictionary.Id, _relationship);

            Response = await GetContributorClient(Guid.NewGuid()).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_sourceWord.Id}/relationships/{_relationship.Id}");
            _view = JsonConvert.DeserializeObject<RelationshipView>(await Response.Content.ReadAsStringAsync());
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