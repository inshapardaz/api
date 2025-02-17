using System.Collections.Generic;
using System.Linq;
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
    public class WhenSearchingCommonWordsLastPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<CommonWordView> _assert;
        private readonly string _searchedWord = "SearchedWords";
        private readonly string _language = new Faker().Random.String2(4);


        public WhenSearchingCommonWordsLastPage()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonWordBuilder.WithLanguage(_language).WithPattern(_searchedWord).Build(50);
            
            _response = await Client.GetAsync($"/tools/{_language}/words?query={_searchedWord}&pageNumber=5&pageSize=10");
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
            _assert.ShouldHaveSelfLink($"/tools/{_language}/words", 5, 10, new KeyValuePair<string, string>("query", _searchedWord));
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
        public void ShouldHavePreviousLinks()
        {
            _assert.ShouldHavePreviousLink($"/tools/{_language}/words", 4, 10, new KeyValuePair<string, string>("query", _searchedWord));
        }

        [Test]
        public void ShouldReturnExpectedData()
        {
            var expectedItems = CommonWordBuilder.Words
                .Where(a => a.Word.Contains(_searchedWord))
                .OrderBy(a => a.Word).Skip(4 * 10).Take(10);
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
