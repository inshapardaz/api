using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticleContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingArticlesContentsWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleContentDto _content;

        public WhenDeletingArticlesContentsWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).WithContent().Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == article.Id);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/articles/{article.Id}/contents?language={_content.Language}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedArticleContent()
        {
            ArticleContentAssert.ShouldHaveDeletedContent(DatabaseConnection, _content);
        }

        [Test]
        public void ShouldHaveDeletedArticleContentFiles()
        {
            var file = ArticleBuilder.Files.Single(x => x.Id == _content.FileId);
            FileAssert.FileDoesnotExist(file);
        }
    }
}
