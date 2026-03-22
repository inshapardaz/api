using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticle
{
    [TestFixture]
    public class WhenDeletingArticleAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var articles = ArticleBuilder.WithLibrary(LibraryId).Build(4);
            var expected = articles.First();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/articles/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
