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
    public class WhenGettingArticlesByAuthorAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;
        private AuthorDto _author;
        private IEnumerable<ArticleDto> _authorArticles;

        public WhenGettingArticlesByAuthorAsReader() 
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorArticles = ArticleBuilder.WithLibrary(LibraryId).WithAuthor(_author).IsPublic().Build(5);
            AuthorBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=1&pageSize=10&authorId={_author.Id}");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 1, 10, 
                new KeyValuePair<string,string>("authorId", _author.Id.ToString()));
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
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveCorrectPagination()
        {
            _assert.ShouldHavePageCount(1)
                    .ShouldHavePageSize(10)
                    .ShouldHavePage(1)
                    .ShouldHaveTotalCount(5);
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(1)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_authorArticles.Count())
                   .ShouldHaveItems(5);
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = _authorArticles.OrderBy(a => a.Title).Take(10).ToArray();
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
                            .ShouldHavePublicImageLink()
                            .ShouldBeSameAs(expected, DatabaseConnection);
            }
        }
    }
}
