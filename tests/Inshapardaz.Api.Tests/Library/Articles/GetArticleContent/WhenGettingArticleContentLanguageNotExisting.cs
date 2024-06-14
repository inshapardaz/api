using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleContent
{
    [TestFixture]
    public class WhenGettingArticleContentLanguageNotExisting
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingArticleContentLanguageNotExisting()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().WithContent().Build();
            var content = ArticleBuilder.Contents.Single(x => x.ArticleId == article.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{article.Id}/contents?language=babel");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
