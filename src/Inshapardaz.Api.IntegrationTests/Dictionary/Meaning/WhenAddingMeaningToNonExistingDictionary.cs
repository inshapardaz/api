using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Meaning
{
    [TestFixture]
    public class WhenAddingMeaningToNonExistingDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Word _word;
        private readonly Guid _userId = Guid.NewGuid();

        private Domain.Entities.Dictionary.Meaning _meaning;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _word = new Domain.Entities.Dictionary.Word
            {
                Id = -2,
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            _meaning = new Domain.Entities.Dictionary.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{-2313}/words/{_word.Id}/meanings", _meaning);
        }
        
        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}