using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Relationships
{
    [TestFixture]
    public class WhenDeletingARelationshipInDictionaryAsAnonymousUser : IntegrationTestBase
    {
        private WordRelation _relation;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _relationId;

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

            var sourceWord = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male
            };

            sourceWord = WordDataHelper.CreateWord(_dictionary.Id, sourceWord);
            _wordId = sourceWord.Id;

            var relatedWord = new Domain.Entities.Dictionary.Word
            {
                Title = "abc2",
                TitleWithMovements = "xyz2",
                Language = Languages.Arabic,
                Pronunciation = "fhjkshf",
                Attributes = GrammaticalType.SiftIshara
            };
            relatedWord = WordDataHelper.CreateWord(_dictionary.Id, relatedWord);


            var relation = new WordRelation
            {
                SourceWordId = sourceWord.Id,
                RelatedWordId = relatedWord.Id,
                RelationType = RelationType.Compound
            };

            relation = RelationshipDataHelper.CreateRelationship(_dictionary.Id, relation);

            _relationId = relation.Id;

            Response = await GetContributorClient(Guid.Empty).DeleteAsync($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/relationships/{_relationId}");

            _relation = RelationshipDataHelper.GetRelationship(_dictionary.Id, _relationId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TranslationDataHelper.DeleteTranslation(_dictionary.Id, _wordId, _relationId);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldnotHaveDeletedTheWord()
        {
            _relation.ShouldNotBeNull();
        }
    }
}