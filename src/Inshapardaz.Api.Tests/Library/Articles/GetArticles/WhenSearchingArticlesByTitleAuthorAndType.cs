using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenSearchingArticlesByTitleAuthorAndType
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;
        private AuthorDto _author;
        private IEnumerable<ArticleDto> _authorArticles;

        public WhenSearchingArticlesByTitleAuthorAndType() 
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorArticles = ArticleBuilder.WithLibrary(LibraryId).WithType(ArticleType.Poetry).WithAuthor(_author).IsPublic().Build(25);
            ArticleBuilder.WithLibrary(LibraryId).WithType(ArticleType.Writing).WithAuthor(_author).IsPublic().Build(30);
            AuthorBuilder.WithLibrary(LibraryId).WithArticles(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=2&pageSize=10&authorId={_author.Id}&query=itle&type=poetry");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 2, 10,
                new KeyValuePair<string, string>("query", "itle"), 
                new KeyValuePair<string, string>("authorid", _author.Id.ToString()),
                new KeyValuePair<string, string>("type", ArticleType.Poetry.ToDescription()));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/articles", 3, 10, 
                new KeyValuePair<string, string>("query", "itle"),
                new KeyValuePair<string, string>("authorid", _author.Id.ToString()),
                new KeyValuePair<string, string>("type", ArticleType.Poetry.ToDescription()));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/articles", 1, 10, 
                new KeyValuePair<string, string>("query", "itle"),
                new KeyValuePair<string, string>("authorid", _author.Id.ToString()),
                new KeyValuePair<string, string>("type", ArticleType.Poetry.ToDescription()));
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = _authorArticles.Where(b => b.Title.Contains("itle") && b.Type == ArticleType.Poetry)
                                            .OrderBy(a => a.Title).Skip(10).Take(10).ToArray();
            _assert.Data.Count().Should().Be(expectedItems.Length); 
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.Type.Should().Be(ArticleType.Poetry.ToDescription());
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
