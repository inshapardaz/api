using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary.WordList
{
    [TestFixture]
    public class WhenGettingWordsForPrivateDictionaryAsDifferentUser : IntegrationTestBase
    {
        private PageView<WordView> _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private List<Domain.Entities.Dictionary.Word> _words;
        private readonly Guid user1 = Guid.NewGuid();
        private readonly Guid user2 = Guid.NewGuid();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary { IsPublic = false, Name = "Test1", UserId = user1};
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);
            var words = new List<Domain.Entities.Dictionary.Word>();

            for (int i = 0; i < 5; i++)
            {
                words.Add(new Domain.Entities.Dictionary.Word {DictionaryId = _dictionary.Id, Title = $"abc {i}", TitleWithMovements = "a$5fv", Attributes = GrammaticalType.FealLazim, Language = Languages.Bangali, Description = "d", Pronunciation = "p"});
            }

            _words = new List<Domain.Entities.Dictionary.Word>();
            foreach (var word in _words)
            {
                _words.Add(WordDataHelper.CreateWord(_dictionary.Id, word));
            }


            Response = await GetContributorClient(user2).GetAsync($"/api/dictionaries/{_dictionary.Id}/words");
            _view = JsonConvert.DeserializeObject<PageView<WordView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary.Id);
            foreach (var word in _words)
            {
                WordDataHelper.DeleteWord(_dictionary.Id, word.Id);
            }
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}