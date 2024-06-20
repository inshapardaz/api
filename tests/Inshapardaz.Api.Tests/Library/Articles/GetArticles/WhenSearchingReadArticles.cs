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
    public class WhenSearchingReadArticles : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;

        public WhenSearchingReadArticles()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ArticleBuilder.WithLibrary(LibraryId).AddToRecentReads(AccountId, 25).Build(40);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=1&pageSize=10&read=true&query=itle");
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 1, 10, 
                new KeyValuePair<string, string>("query", "itle"),
                new KeyValuePair<string, string>("read", "true"));
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = ArticleBuilder.Articles.Where(b => ArticleBuilder.RecentReads.Any(f => f.ArticleId ==  b.Id ))
                                              .Where(a => a.Title.Contains("itle"))
                                              .OrderBy(a => a.Title).Take(10)
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
