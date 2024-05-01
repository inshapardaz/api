using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingArticlesOrderByLastModified : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;

        public WhenGettingArticlesOrderByLastModified() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ArticleBuilder.WithLibrary(LibraryId).IsPublic().Build(10);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=1&pageSize=12&sortBy=lastmodified");

            _assert = new PagingAssert<ArticleView>(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 1, 12,
                new KeyValuePair<string, string>("sortBy", "lastModified"));
        }

        [Test]
        public void ShouldHaveCorrectPaginationData()
        {
            _assert.ShouldHavePageCount(1)
                .ShouldHavePageSize(12)
                .ShouldHavePage(1)
                .ShouldHaveTotalCount(10);

        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = ArticleBuilder.Articles.OrderBy(a => a.LastModified).ThenBy(x => x.Title).ToArray();
            _assert.Data.Count().Should().Be(expectedItems.Length);
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                ;
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.ShouldMatch(expected, DatabaseConnection, LibraryId)
                            .ShouldHaveSelfLink()
                            .WithReadOnlyLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveAddFavoriteLink()
                            .ShouldHavePublicImageLink()
                            .ShouldBeSameAs(expected, DatabaseConnection);
            }
        }
    }
}
