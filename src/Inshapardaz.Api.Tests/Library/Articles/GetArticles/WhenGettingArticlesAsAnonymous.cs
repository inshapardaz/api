using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingArticlesAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;
        private IEnumerable<CategoryDto> _categories;


        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2);

            ArticleBuilder.WithLibrary(LibraryId)
                .IsPublic()
                .WithCategories(_categories)
                .WithContents(2)
                .Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldHaveCorrectPagination()
        {
            _assert.ShouldHavePageCount(1)
                    .ShouldHavePageSize(10)
                    .ShouldHavePage(1)
                    .ShouldHaveTotalCount(4);
        }

        [Test]
        public void ShouldReturnExpectedArticle()
        {
            var expectedItems = ArticleBuilder.Articles.OrderBy(a => a.Title).Take(10).ToArray();
            _assert.Data.Count().Should().Be(expectedItems.Length);
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.ShouldMatch(expected, DatabaseConnection, LibraryId)
                            .ShouldHaveSelfLink()
                            .WithReadOnlyLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldBeSameCategories(_categories)
                            .ShouldHavePublicImageLink()
                            .ShouldNotHaveAddFavoriteLink()
                            .ShouldNotHaveRemoveFavoriteLink()
                            .ShouldHaveContents(
                                ArticleBuilder.Contents.Where(c => c.ArticleId == actual.Id).ToList(),
                                false)
                            .ShouldBeSameAs(expected, DatabaseConnection);
            };
        }
    }
}
