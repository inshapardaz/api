using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticleToFavorite
{
    [TestFixture]
    public class WhenAddArticleToFavoriteAlreadyInFavorite : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
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

            _response = await Client.PostObject($"/libraries/{LibraryId}/favorites/articles/{_article.Id}", new object());
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
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
            _assert.ShouldBeAddedToFavorite(_article.Id, AccountId);
        }
    }
}
