using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.RemoveArtiucleFromFavorites
{
    [TestFixture]
    public class WhenRemoveArticleFromFavoriteThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private int _articleId = -RandomData.Number;

        public WhenRemoveArticleFromFavoriteThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/favorites/articles/{_articleId}");
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
            ArticleAssert.ShouldNotBeInFavorites(_articleId, AccountId, DatabaseConnection);
        }
    }
}
