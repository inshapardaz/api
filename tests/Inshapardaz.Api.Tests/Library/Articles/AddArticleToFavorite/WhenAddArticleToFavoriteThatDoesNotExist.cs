using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticleToFavorite
{
    [TestFixture]
    public class WhenAddArticleToFavoriteThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddArticleToFavoriteThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject<object>($"/libraries/{LibraryId}/favorites/articles/{-RandomData.Number}", new object());
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldBeOk()
        {
            _response.ShouldBeOk();
        }
    }
}
