using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.RemoveArtiucleFromFavorites
{
    [TestFixture]
    public class WhenRemoveNonFavoriteArticleFromFavorite : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private ArticleDto _article;

        public WhenRemoveNonFavoriteArticleFromFavorite()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var articles = ArticleBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .Build(2);

            _article = articles.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/favorites/articles/{_article.Id}");
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
        public void ShouldBeRemovedFromFavorites()
        {
            _assert.ShouldNotBeInFavorites(_article.Id, AccountId);
        }
    }
}
