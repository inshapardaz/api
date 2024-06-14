using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenSearchinArticlesWithMultipleAuthorsAndCategories : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;
        private IEnumerable<CategoryDto> _categories;
        private IEnumerable<AuthorDto> _authors;

        public WhenSearchinArticlesWithMultipleAuthorsAndCategories()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2);
            _authors = AuthorBuilder.WithLibrary(LibraryId).Build(2);

            ArticleBuilder.WithLibrary(LibraryId).WithCategories(_categories).WithAuthors(_authors).IsPublic().Build(30);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=2&pageSize=10&query=itle");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 2, 10);
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/articles", 3, 10);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/articles", 1, 10);
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = ArticleBuilder.Articles.Where(b => b.Title.Contains("itle"))
                                              .OrderBy(a => a.Title).Skip(10).Take(10)
                                              .ToArray();
            _assert.Data.Count().Should().Be(expectedItems.Length);
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.ShouldMatch(expected, DatabaseConnection, LibraryId)
                            .ShouldHaveSelfLink()
                            .WithReadOnlyLinks()
                            .ShouldHavePublicImageLink()
                            .ShouldHaveAddFavoriteLink()
                            .ShouldBeSameAs(expected, DatabaseConnection);
            }
        }
    }
}
