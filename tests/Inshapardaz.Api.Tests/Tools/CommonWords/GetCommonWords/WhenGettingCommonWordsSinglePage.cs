﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.CommonWords.GetCommonWords
{
    [TestFixture]
    public class WhenGettingCommonWordsSinglePage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<CommonWordView> _assert;
        private readonly string _language = new Faker().Random.String2(4);

        public WhenGettingCommonWordsSinglePage()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).Build(4);
            
            _response = await Client.GetAsync($"/tools/{_language}/words");
            _assert = Services.GetService<PagingAssert<CommonWordView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/tools/{_language}/words");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedData()
        {
            var expectedItems = CommonWordBuilder.Words.OrderBy(a => a.Word).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<CommonWordAssert>().ForView(actual)
                    .ShouldBeSameAs(item)
                    .ShouldNotHaveUpdateLink()
                    .ShouldNotHaveDeleteLink();
            }
        }
    }
}
