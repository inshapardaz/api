using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Relationships
{
    [TestFixture]
    public class WhenAddingRelationToNonExistingDictionary : IntegrationTestBase
    {
        private readonly Guid _userId = Guid.NewGuid();

        private WordRelation _relation;

        [OneTimeSetUp]
        public async Task Setup()
        {
            
            _relation = new WordRelation
            {
                RelatedWordId = -1,
                SourceWordId = -2,
                RelationType = RelationType.HalatTakhsis
            };

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{-2313}/words/{2}/relationships", _relation);
        }
        
        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}