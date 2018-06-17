﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Dictionary
{
    [TestFixture]
    public class WhenGettingDictionariesAnonymously : IntegrationTestBase
    {
        private DictionariesView _view;
        private Domain.Entities.Dictionary _dictionary1;
        private Domain.Entities.Dictionary _dictionary2;
        private Domain.Entities.Dictionary _dictionary3;

        [OneTimeSetUp]
        public async Task Setup()
        {

            _dictionary1 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = true, Name = "Test1"});
            _dictionary2 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = true, Name = "Test2"});
            _dictionary3 = DictionaryDataHelper.CreateDictionary(new Domain.Entities.Dictionary { IsPublic = false, Name = "Test3", UserId = Guid.NewGuid()});

            Response = await GetClient().GetAsync("/api/dictionaries");
            _view = JsonConvert.DeserializeObject<DictionariesView>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            DictionaryDataHelper.DeleteDictionary(_dictionary1.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary2.Id);
            DictionaryDataHelper.DeleteDictionary(_dictionary3.Id);
        }

        [Test]
        public void ShouldReturn200()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldHaveCorrectResponseBody()
        {
            Assert.That(_view, Is.Not.Null);
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.ShouldContain(l => l.Rel == RelTypes.Self && l.Href != null);
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _view.Links.ShouldNotContain(l => l.Rel == RelTypes.Create && l.Href != null);
        }

        [Test]
        public void ShouldReturnPublicDictionaries()
        {
            _view.Items.Count().ShouldBeGreaterThanOrEqualTo(2);
            _view.Items.ShouldAllBe(d => d.IsPublic);
        }

        [Test]
        public void ShouldNotReturnPrivateDictionaries()
        {
            _view.Items.ShouldNotContain(d => d.IsPublic == false);
        }
    }
}