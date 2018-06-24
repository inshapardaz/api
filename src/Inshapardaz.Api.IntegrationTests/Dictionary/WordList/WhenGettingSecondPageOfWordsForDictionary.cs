using System.Collections.Generic;
using System.Linq;
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
    public class WhenGettingSecondPageOfWordsForDictionary : IntegrationTestBase
    {
        private PageView<WordView> _view;
        private Domain.Entities.Dictionary.Dictionary _dictionary;
        private List<Domain.Entities.Dictionary.Word> _words;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary.Dictionary { IsPublic = true, Name = "Test1"};
            _dictionary = DictionaryDataHelper.CreateDictionary(_dictionary);

            var words = new List<Domain.Entities.Dictionary.Word>();

            for (int i = 0; i < 21; i++)
            {
                words.Add(new Domain.Entities.Dictionary.Word {DictionaryId = _dictionary.Id, Title = $"abc {i}", TitleWithMovements = "a$5fv", Attributes = GrammaticalType.FealLazim, Language = Languages.Bangali, Description = "d", Pronunciation = "p"});
            }

            _words = new List<Domain.Entities.Dictionary.Word>();
            foreach (var word in words)
            {
                _words.Add(WordDataHelper.CreateWord(_dictionary.Id, word));
            }


            Response = await GetClient().GetAsync($"/api/dictionaries/{_dictionary.Id}/words?pageNumber=2");
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
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldRetrunSelfLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
        }

        [Test]
        public void ShouldRetrunNextLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Next && l.Href != null);
        }

        [Test]
        public void ShouldRetrunPreviousLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Previous && l.Href != null);
        }

        [Test]
        public void ShouldContainAllWordsInThePage()
        {
            _view.Data.Count().ShouldBe(10);
        }
    }
}