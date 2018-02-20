﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.WordList
{
    [TestFixture]
    public class WhenGettingLastPageOfWordsForDictionary : IntegrationTestBase
    {
        private PageView<WordView> _view;
        private Domain.Entities.Dictionary _dictionary;
        private List<Domain.Entities.Word> _words;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = new Domain.Entities.Dictionary {Id = -1, IsPublic = true, Name = "Test1"};
            _words = new List<Domain.Entities.Word>();

            for (int i = 0; i < 21; i++)
            {
                _words.Add(new Domain.Entities.Word {DictionaryId = _dictionary.Id, Id = i, Title = $"abc {i}", TitleWithMovements = "a$5fv", Attributes = GrammaticalType.FealLazim, Language = Languages.Bangali, Description = "d", Pronunciation = "p"});
            }

            DictionaryDataHelper.CreateDictionary(_dictionary);
            foreach (var word in _words)
            {
                WordDataHelper.CreateWord(_dictionary.Id, word);
            }

            DictionaryDataHelper.RefreshIndex();

            Response = await GetClient().GetAsync($"/api/dictionaries/{_dictionary.Id}/words?pageNumber=3");
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
        public void ShouldNotRetrunNextLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Next && l.Href != null);
        }

        [Test]
        public void ShouldRetrunPreviousLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Previous && l.Href != null);
        }

        [Test]
        public void ShouldContainAllWordsInThePage()
        {
            _view.Data.Count().ShouldBe(1);
        }
    }
}