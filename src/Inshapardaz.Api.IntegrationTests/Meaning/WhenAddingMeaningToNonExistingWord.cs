using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Meaning
{
    [TestFixture]
    public class WhenAddingMeaningToNonExistingWord : IntegrationTestBase
    {
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Meaning _meaning;
        private Domain.Entities.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {

            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                UserId = _userId
            };
            _meaning = new Domain.Entities.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            DictionaryDataHelper.CreateDictionary(_dictionary);

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{_dictionary.Id}/words/-34/meanings", _meaning);
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