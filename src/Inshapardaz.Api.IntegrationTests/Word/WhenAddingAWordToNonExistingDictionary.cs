using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Word
{
    [TestFixture]
    public class WhenAddingAWordToNonExistingDictionary : IntegrationTestBase
    {
        private WordView _word;
        private readonly Guid _userId = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _word = new WordView
            {
                Id = -2,
                Title = "abc",
                TitleWithMovements = "xyz",
                LanguageId = (int)Languages.Bangali,
                Pronunciation = "pas",
                AttributeValue = (int)GrammaticalType.FealImdadi & (int)GrammaticalType.Male,
            };

            Response = await GetContributorClient(_userId).PostJson($"/api/dictionaries/{int.MinValue}/words", _word);
        }
        
        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}