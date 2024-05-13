using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleById
{
    [TestFixture]
    public class WhenGettingArticleByIdThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingArticleByIdThatDoesNotExist()
            :base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{RandomData.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
