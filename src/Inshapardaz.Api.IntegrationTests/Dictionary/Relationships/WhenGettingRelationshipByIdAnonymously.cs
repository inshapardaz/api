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

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Relationships
{
    [TestFixture]
    public class WhenGettingRelationshipByIdAnonymously : IntegrationTestBase
    {
        private RelationshipView _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private WordRelation _relationship;
        private Domain.Entities.Dictionary.Word _sourceWord;
        private Domain.Entities.Dictionary.Word _relatedWord;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = true,
                UserId = _userId,
                Name = "Test1"
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _sourceWord = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male
            };

            _sourceWord = WordDataHelper.CreateWord(_dictionary.Id, _sourceWord);

            _relatedWord = new Domain.Entities.Dictionary.Word
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

            Response = await GetContributorClient(Guid.Empty).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_sourceWord.Id}/relationships/{_relationship.Id}");
            _view = JsonConvert.DeserializeObject<RelationshipView>(await Response.Content.ReadAsStringAsync());
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
        public void ShouldReturnTranslationWithCorrectId()
        {
            _view.Id.ShouldBe(_relationship.Id);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectSourceWordId()
        {
            _view.SourceWordId.ShouldBe(_relationship.SourceWordId);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectRelatedWordId()
        {
            _view.RelatedWordId.ShouldBe(_relationship.RelatedWordId);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectSourceWord()
        {
            _view.SourceWord.ShouldBe(_sourceWord.Title);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectRelatedWord()
        {
            _view.RelatedWord.ShouldBe(_relatedWord.Title);
        }

        [Test]
        public void ShouldReturnTranslationWithCorrectRelationshipType()
        {
            _view.RelationTypeId.ShouldBe((int)_relationship.RelationType);
        }


        [Test]
        public void ShouldReturnTranslationWithRelationName()
        {
            _view.RelationType.ShouldNotBeNull();
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnUpdateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Update);
        }

        [Test, Ignore("Security not implemented completely")]
        public void ShouldNotReturnDeleteLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Delete);
        }

        [Test]
        public void ShouldReturnWordLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Word);
        }

        [Test]
        public void ShouldReturnRelatedWordLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.RelatedWord);
        }
    }
}