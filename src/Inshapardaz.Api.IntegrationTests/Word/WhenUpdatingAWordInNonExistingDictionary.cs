using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Word
{
    [TestFixture]
    public class WhenUpdatingAWordInNonExistingDictionary : IntegrationTestBase
    {
        private Domain.Entities.Word _word;
        private readonly Guid _userId = Guid.NewGuid();

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

            _word.Title += "updated";
            _word.TitleWithMovements += "updated";
            _word.Language = Languages.Pali;
            _word.Attributes = GrammaticalType.HarfSoot;
            _word.Description += "updated";
            _word.Pronunciation += "updated";

            Response = await GetContributorClient(_userId).PutJson($"/api/dictionaries/{-243243}/words/{_word.Id}", _word.Map());
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}