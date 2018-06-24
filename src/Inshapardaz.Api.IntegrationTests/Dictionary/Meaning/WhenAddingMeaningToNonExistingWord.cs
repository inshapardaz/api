using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Meaning
{
    [TestFixture]
    public class WhenAddingMeaningToNonExistingWord : IntegrationTestBase
    {
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Dictionary.Meaning _meaning;
        private Domain.Entities.Dictionary.Dictionary _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                UserId = _userId
            };
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            _meaning = new Domain.Entities.Dictionary.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

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