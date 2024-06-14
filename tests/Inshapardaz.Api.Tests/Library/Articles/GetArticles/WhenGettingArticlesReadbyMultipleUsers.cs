using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingArticlesReadbyMultipleUsers : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;

        public WhenGettingArticlesReadbyMultipleUsers() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account2 = AccountBuilder.As(Role.Reader).Build();
            ArticleBuilder.WithLibrary(LibraryId)
                    .WithCategories(2)
                    .WithAuthors(2)
                    .AddToFavorites(AccountId)
                    .AddToRecentReads(AccountId)
                    .AddToFavorites(account2.Id)
                    .AddToRecentReads(account2.Id)
                    .WithContent()
                    .IsPublic()
                    .Build(20);
            

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?pageNumber=1&pageSize=12");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 1, 12);
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/articles", 2, 12);
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = ArticleBuilder.Articles.OrderBy(a => a.Title).Take(12).ToArray();
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
                            .ShouldHaveRemoveFavoriteLink()
                            .ShouldHaveContents(
                                ArticleBuilder.Contents.Where(c => c.ArticleId == actual.Id).ToList(),
                                false)
                            .ShouldBeSameAs(expected, DatabaseConnection);
            }
        }
    }
}
