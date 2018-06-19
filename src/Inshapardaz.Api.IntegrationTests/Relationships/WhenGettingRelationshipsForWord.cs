using System;
using System.Collections.Generic;
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
    public class WhenGettingRelationshipsForWord : IntegrationTestBase
    {
        private IEnumerable<RelationshipView> _view;
        private Domain.Entities.Dictionary _dictionary;
        private Domain.Entities.Word _word;
        private Domain.Entities.Word _word2;
        private Domain.Entities.Word _word3;
        private readonly Guid _userId = Guid.NewGuid();
        private WordRelation _relation1;
        private WordRelation _relation2;
        private WordRelation _relation3;

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

            _word2 = new Domain.Entities.Word
            {
                Title = "AbcDeFg",
                TitleWithMovements = "xyz",
                Language = Languages.Avestan,
                Pronunciation = "uwweui",
                Attributes = GrammaticalType.HarfNafi & GrammaticalType.Female,
                DictionaryId = _dictionary.Id
            };

            _word3 = new Domain.Entities.Word
            {
                Title = "mnoPQr",
                TitleWithMovements = "mUrerer",
                Language = Languages.Avestan,
                Pronunciation = "gretrt",
                Attributes = GrammaticalType.FealNakis,
                DictionaryId = _dictionary.Id
            };


            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);
            _word = WordDataHelper.CreateWord(_dictionary.Id, _word);
            _word2 = WordDataHelper.CreateWord(_dictionary.Id, _word2);
            _word3 = WordDataHelper.CreateWord(_dictionary.Id, _word3);

            _relation1 = RelationshipDataHelper.CreateRelationship(_dictionary.Id, new WordRelation {  SourceWordId = _word.Id, RelatedWordId = _word2.Id, RelationType = RelationType.Halat }); 
            _relation2 = RelationshipDataHelper.CreateRelationship(_dictionary.Id, new WordRelation { SourceWordId = _word.Id, RelatedWordId = _word3.Id, RelationType = RelationType.Halat });
            _relation3 = RelationshipDataHelper.CreateRelationship(_dictionary.Id, new WordRelation { SourceWordId = _word2.Id, RelatedWordId = _word3.Id, RelationType = RelationType.Halat });

            Response = await GetContributorClient(_userId).GetAsync($"/api/dictionaries/{_dictionary.Id}/words/{_word.Id}/relationships");
            _view = JsonConvert.DeserializeObject<IEnumerable<RelationshipView>>(await Response.Content.ReadAsStringAsync());
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
        public void ShouldReturnTranslations()
        {
            _view.ShouldNotBeEmpty();
        }

        [Test]
        public void ShouldContainTranslationForTheWord()
        {
            _view.ShouldContain(m => m.Id == _relation1.Id);
            _view.ShouldContain(m => m.Id == _relation2.Id);
        }

        [Test]
        public void ShouldNotContainTranslationForOtherWord()
        {
            _view.ShouldNotContain(m => m.Id == _relation3.Id);
        }

    }
}