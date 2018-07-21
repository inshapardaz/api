﻿using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Relationships
{
    [TestFixture]
    public class WhenAddingRelationshipToOtherUsersPrivateDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();
        private RelationshipView _relationshipView;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                UserId = _userId1,
                Name = "Test1"
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _word = new Domain.Entities.Dictionary.Word
            {
                DictionaryId = _dictionary.Id,
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };
            _word = WordDataHelper.CreateWord(_dictionary.Id, _word);

            _relationshipView = new RelationshipView
            {
                SourceWordId = -1,
                RelatedWordId = -2,
                RelationTypeId = 4
            };

            Response = await GetContributorClient(_userId2).PostJson($"/api/dictionaries/{_dictionary.Id}/words/-34/relationships", _relationshipView);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _word.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}