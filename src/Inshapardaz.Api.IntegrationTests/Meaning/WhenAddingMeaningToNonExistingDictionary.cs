using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Meaning
{
    [TestFixture]
    public class WhenAddingMeaningToNonExistingDictionary : IntegrationTestBase
    {
        private Domain.Entities.Word _word;
        private readonly Guid _userId = Guid.NewGuid();
        private Domain.Entities.Meaning _meaning;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _word = new Domain.Entities.Word
            {
                Id = -2,
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            _meaning = new Domain.Entities.Meaning
            {
                Context = "default",
                Value = "meaning value",
                Example = "example text"
            };

            WordDataHelper.CreateWord(-434, _word);

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{-2}/words/{_word.Id}/meanings", _meaning);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            WordDataHelper.DeleteWord(-434, _word.Id);
        }
        
        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}