using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Relationships
{
    [TestFixture]
    public class WhenUpdatingARelationshipInPrivateDictionaryOfAnotherUser: IntegrationTestBase
    {
        private WordRelation _relation;
        private Domain.Entities.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _relationshipId;
        private WordRelation _oldRelationship;

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

            var sourceWord = new Domain.Entities.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male
            };

            sourceWord = WordDataHelper.CreateWord(_dictionary.Id, sourceWord);
            _wordId = sourceWord.Id;

            var relatedWord = new Domain.Entities.Word
            {
                Title = "abc2",
                TitleWithMovements = "xyz2",
                Language = Languages.Arabic,
                Pronunciation = "fhjkshf",
                Attributes = GrammaticalType.SiftIshara
            };
            relatedWord = WordDataHelper.CreateWord(_dictionary.Id, relatedWord);


            _oldRelationship = new WordRelation
            {
                SourceWordId = sourceWord.Id,
                RelatedWordId = relatedWord.Id,
                RelationType = RelationType.Compound
            };

            _oldRelationship = RelationshipDataHelper.CreateRelationship(_dictionary.Id, _oldRelationship);

            _relationshipId = _oldRelationship.Id;

            _oldRelationship.RelationType = RelationType.OppositeGender;

            Response = await GetContributorClient(Guid.NewGuid()).PutJson($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/relationships/{_relationshipId}", _oldRelationship.Map());

            _relation = RelationshipDataHelper.GetRelationship(_dictionary.Id, _relationshipId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TranslationDataHelper.DeleteTranslation(_dictionary.Id, _wordId, _relationshipId);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotHaveUpdateTheTranslation()
        {
            _relation.SourceWordId.ShouldBe(_oldRelationship.SourceWordId);
            _relation.RelatedWordId.ShouldBe(_oldRelationship.RelatedWordId);
            _relation.RelationType.ShouldNotBe(_oldRelationship.RelationType);
        }
    }
}