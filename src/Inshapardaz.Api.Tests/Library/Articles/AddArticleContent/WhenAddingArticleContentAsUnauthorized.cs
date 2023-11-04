using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticleContent
{
    [TestFixture]
    public class WhenAddingArticleContentAsUnauthorized
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostString($"/libraries/{LibraryId}/articles/{article.Id}/contents", RandomData.Text, "hi");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
