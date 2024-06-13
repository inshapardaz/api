using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleImage
{
    [TestFixture]
    public class WhenUploadingArticleImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _articleAssert;
        private long _articleId;

        public WhenUploadingArticleImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).WithNoImage().Build();
            _articleId = article.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/articles/{_articleId}/image", RandomData.Bytes);
            _articleAssert = ArticleAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            ArticleBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveHttpResponseMessage()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _articleAssert.ShouldHaveCorrectImageLocationHeader(_articleId);
        }

        [Test]
        public void ShouldHaveAddedImageToArticle()
        {
            ArticleAssert.ShouldHaveAddedArticleImage(_articleId, DatabaseConnection, FileStore);
        }
    }
}
