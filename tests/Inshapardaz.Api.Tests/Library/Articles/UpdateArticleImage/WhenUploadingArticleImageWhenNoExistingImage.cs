using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleImage
{
    [TestFixture]
    public class WhenUploadingArticleImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
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
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
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
            _assert.ShouldHaveCorrectImageLocationHeader(_articleId);
        }

        [Test]
        public void ShouldHaveAddedImageToArticle()
        {
            _assert.ShouldHaveAddedArticleImage(_articleId);
        }
    }
}
