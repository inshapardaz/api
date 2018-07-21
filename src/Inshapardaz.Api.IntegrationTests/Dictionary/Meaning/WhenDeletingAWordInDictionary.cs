﻿using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Meaning
{
    [TestFixture]
    public class WhenDeletingAMeaningInDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Meaning _meaning;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private long _wordId;
        private long _meaningId;

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

            var word = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            word = WordDataHelper.CreateWord(_dictionary.Id, word);
            _wordId = word.Id;

            var meaning = new Domain.Entities.Dictionary.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            meaning = MeaningDataHelper.CreateMeaning(_dictionary.Id, _wordId, meaning);

            _meaningId = meaning.Id;

            Response = await GetContributorClient(_userId).DeleteAsync($"api/dictionaries/{_dictionary.Id}/words/{_wordId}/meanings/{_meaningId}");

            _meaning = MeaningDataHelper.GetMeaning(_dictionary.Id, word.Id, _meaningId);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            MeaningDataHelper.DeleteMeaning(_dictionary.Id, _wordId, _meaningId);
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Test]
        public void ShouldHaveDeletedTheWord()
        {
            _meaning.ShouldBeNull();
        }
    }
}