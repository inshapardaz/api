using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private ArticleContentAssert _assert;
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
            _assert = Services.GetService<ArticleContentAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveDeletedContent(_content);
        }

        [Test]
        public void ShouldHaveDeletedArticleContentFiles()
        {
            var file = ArticleBuilder.Files.Single(x => x.Id == _content.FileId);
            Services.GetService<FileStoreAssert>()
                .FileDoesnotExist(file);
        }
    }
}
