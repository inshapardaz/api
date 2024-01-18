using Inshapardaz.Api.Tests.Asserts;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticleContent
{
    [TestFixture]
    public class WhenDeletingArticleContentsAsUnauthorized
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).WithContent().Build();
            var content = ArticleBuilder.Contents.Single(x => x.ArticleId == article.Id);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/articles/{article.Id}/contents?language={content.Language}");
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
