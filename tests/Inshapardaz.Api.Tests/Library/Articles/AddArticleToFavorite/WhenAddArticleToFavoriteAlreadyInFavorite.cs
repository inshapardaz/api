using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticleToFavorite
{
    [TestFixture]
    public class WhenAddArticleToFavoriteAlreadyInFavorite : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;

        public WhenAddArticleToFavoriteAlreadyInFavorite()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var articles = ArticleBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .AddToFavorites(AccountId)
                                    .Build(2);

            _article = articles.PickRandom();

            _response = await Client.PostObject<object>($"/libraries/{LibraryId}/favorites/articles/{_article.Id}", new object());
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldBeAddedToFavorites()
        {
            ArticleAssert.ShouldBeAddedToFavorite(_article.Id, AccountId, DatabaseConnection);
        }
    }
}
