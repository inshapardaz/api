using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticleContent
{
    [TestFixture]
    public class WhenDeletingArticleContentsAsReader() : TestBase(Role.Reader)
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
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
