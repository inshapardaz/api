using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticleContent
{
    [TestFixture]
    public class WhenDeletingArticleContentThatDoesNotExists
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingArticleContentThatDoesNotExists()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/articles/{article.Id}/contents?language={RandomData.Locale}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContentResult()
        {
            _response.ShouldBeNoContent();
        }
    }
}
