using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingArticlesByCategory : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;
        private CategoryDto _category1, _category2;

        public WhenGettingArticlesByCategory() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category1 = CategoryBuilder.WithLibrary(LibraryId).Build();
            _category2 = CategoryBuilder.WithLibrary(LibraryId).Build();
            
            ArticleBuilder.WithLibrary(LibraryId).WithCategory(_category1).Build(30);
            ArticleBuilder.WithLibrary(LibraryId).WithCategory(_category2).Build(15);
            
            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=2&pageSize=5&categoryId={_category2.Id}");
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
                    new KeyValuePair<string, string>("categoryId", _category2.Id.ToString()));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/articles", 3, 5,
                new KeyValuePair<string, string>("categoryId", _category2.Id.ToString()));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/articles", 1, 5,
                new KeyValuePair<string, string>("categoryId", _category2.Id.ToString()));
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var allTagArticles = ArticleBuilder.ArticleCategories.Where(x => x.Value.Contains(_category2.Id)).Select(x => x.Key).ToList();
            var expectedItems = ArticleBuilder.Articles.Where(b => allTagArticles.Contains(b.Id))
                .OrderBy(a => a.Title).Skip(5).Take(5)
                .ToArray();
            _assert.Data.Count().Should().Be(expectedItems.Length);
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<ArticleAssert>().ForArticleView(actual).ForLibrary(LibraryId)
                            .ShouldHaveSelfLink()
                            .WithReadOnlyLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveAddFavoriteLink()
                            .ShouldHavePublicImageLink()
                            .ShouldBeSameAs(expected);
            }
        }
    }
}
