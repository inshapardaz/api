using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleImage
{
    [TestFixture]
    public class WhenUploadingArticleImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private long _articleId;
        private byte[] _oldImage;

        public WhenUploadingArticleImageAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).Build();
            _articleId = article.Id;
            var imageUrl = ArticleTestRepository.GetArticleImageUrl(_articleId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

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
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedArticleImage()
        {
            _assert.ShouldNotHaveUpdatedArticleImage(_articleId, _oldImage);
        }
    }
}
