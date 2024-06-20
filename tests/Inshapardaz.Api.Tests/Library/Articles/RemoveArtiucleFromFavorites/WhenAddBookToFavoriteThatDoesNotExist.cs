using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.RemoveArtiucleFromFavorites
{
    [TestFixture]
    public class WhenRemoveArticleFromFavoriteThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private int _articleId = -RandomData.Number;

        public WhenRemoveArticleFromFavoriteThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/favorites/articles/{_articleId}");
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldNotBeInFavorites()
        {
            _assert.ShouldNotBeInFavorites(_articleId, AccountId);
        }
    }
}
