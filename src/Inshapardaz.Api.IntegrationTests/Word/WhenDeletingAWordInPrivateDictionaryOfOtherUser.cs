using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Word
{
    [TestFixture]
    public class WhenDeletingAWordInPrivateDictionaryOfOtherUser : IntegrationTestBase
    {
        private Domain.Entities.Word _word;
        private Domain.Entities.Dictionary _dictionary;
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();
        private int _wordId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary
            {
                Id = -1,
                IsPublic = false,
                UserId = _userId1,
                Name = "Test1"
            };

            _wordId = -2;
            var word = new Domain.Entities.Word
            {
                Id = _wordId,
                Title = "abc",
                TitleWithMovements = "xyz",
                Language = Languages.Bangali,
                Pronunciation = "pas",
                Attributes = GrammaticalType.FealImdadi & GrammaticalType.Male,
            };

            DictionaryDataHelper.CreateDictionary(_dictionary);
            WordDataHelper.CreateWord(_dictionary.Id, word);

            word.Title += "updated";
            word.TitleWithMovements += "updated";
            word.Language = Languages.Pali;
            word.Attributes = GrammaticalType.HarfSoot;
            word.Description += "updated";
            word.Pronunciation += "updated";

            Response = await GetContributorClient(_userId2).DeleteAsync($"/api/dictionaries/{_dictionary.Id}/words/{word.Id}");

            _word = WordDataHelper.GetWord(_dictionary.Id, word.Id);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            WordDataHelper.DeleteWord(_dictionary.Id, _wordId);
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldNotHaveDeletedTheWord()
        {
            _word.ShouldNotBeNull();
        }
    }
}