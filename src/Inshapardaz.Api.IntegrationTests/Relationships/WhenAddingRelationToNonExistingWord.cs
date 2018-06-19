using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Relationships
{
    [TestFixture]
    public class WhenAddingRelationToNonExistingWord : IntegrationTestBase
    {
        private readonly Guid _userId = Guid.NewGuid();
        private RelationshipView _relationshipView;
        private Domain.Entities.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                UserId = _userId
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _relationshipView = new RelationshipView
            {
                SourceWordId = -1,
                RelatedWordId = -2,
                RelationTypeId  =  1
            };

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{_dictionary.Id}/words/-34/relationships", _relationshipView);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}