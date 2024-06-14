using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleContent
{
    [TestFixture (Ignore ="Better auth should fix it")]
    public class WhenGettingPrivateArticleContentAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic(false).WithContent().Build();
            var content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{_article.Id}/contents?language={content.Language}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnathorised()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
