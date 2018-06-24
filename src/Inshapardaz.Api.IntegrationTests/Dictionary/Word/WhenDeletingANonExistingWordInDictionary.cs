using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.Word
{
    [TestFixture]
    public class WhenDeletingANonExistingWordInDictionary : IntegrationTestBase
    {
        private Domain.Entities.Dictionary.Word _word;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private readonly Guid _userId = Guid.NewGuid();
        private int _wordId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary
            {
                IsPublic = false,
                UserId = _userId,
                Name = "Test1"
            };

            var word = new Domain.Entities.Dictionary.Word
            {
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            word.Title += "updated";
            word.TitleWithMovements += "updated";
            word.Language = Languages.Pali;
            word.Attributes = GrammaticalType.HarfSoot;
            word.Description += "updated";
            word.Pronunciation += "updated";

            Response = await GetContributorClient(_userId).DeleteAsync($"/api/dictionaries/{_dictionary.Id}/words/{word.Id}");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }
    }
}