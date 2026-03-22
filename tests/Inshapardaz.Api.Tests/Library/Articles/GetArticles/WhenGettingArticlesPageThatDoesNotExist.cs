using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingArticlesPageThatDoesNotExist() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            ArticleBuilder.WithLibrary(LibraryId).IsPublic().Build(20);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=5&pageSize=10");
            _assert = Services.GetService<PagingAssert<ArticleView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSelfLink() => _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 5);

        [Test]
        public void ShouldHaveCorrectPaginationData()
        {
            _assert.ShouldHavePageCount(2)
                .ShouldHavePageSize(10)
                .ShouldHavePage(5)
                .ShouldHaveTotalCount(20);

        }

        [Test]
        public void ShouldHaveNextLink() => _assert.ShouldNotHaveNextLink();

        [Test]
        public void ShouldNotHavePreviousLink() => _assert.ShouldNotHavePreviousLink();

        [Test]
        public void ShouldReturnExpectedArticles() => _assert.ShouldHaveNoData();
    }
}
