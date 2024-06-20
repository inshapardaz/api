using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingingArticlesByType : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;

        public WhenGettingingArticlesByType()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ArticleBuilder.WithLibrary(LibraryId).WithType(Domain.Models.Library.ArticleType.Poetry).Build(30);
            ArticleBuilder.WithLibrary(LibraryId).WithType(Domain.Models.Library.ArticleType.Writing).Build(10);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=2&pageSize=10&type=poetry");
            _assert = Services.GetService<PagingAssert<ArticleView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles",
                new KeyValuePair<string, string>("type", "poetry"));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/articles", 3, 10, 
                new KeyValuePair<string, string>("type", "poetry"));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/articles", 1, 10,
                new KeyValuePair<string, string>("type", "poetry"));
        }

        [Test]
        public void ShouldHaveCorrectPagination()
        {
            _assert.ShouldHavePageCount(3)
                    .ShouldHavePageSize(10)
                    .ShouldHavePage(2)
                    .ShouldHaveTotalCount(30);
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = ArticleBuilder.Articles.Where(b => b.Type == Domain.Models.Library.ArticleType.Poetry)
                                              .OrderBy(a => a.Title).Skip(10).Take(10)
                                              .ToArray();
            _assert.Data.Count().Should().Be(expectedItems.Length);
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<ArticleAssert>().ForArticleView(actual).ForLibrary(LibraryId)
                            .ShouldHaveSelfLink()
                            .WithWriteableLinks()
                            .ShouldHaveImageUpdateLink()
                            .ShouldHaveAddContentLink()
                            .ShouldHavePublicImageLink()
                            .ShouldHaveAddFavoriteLink()
                            .ShouldBeSameAs(expected);
            }
        }
    }
}
