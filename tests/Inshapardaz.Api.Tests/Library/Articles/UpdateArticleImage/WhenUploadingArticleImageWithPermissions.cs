using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUploadingArticleImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private long _articleId;
        private byte[] _newImage = RandomData.Bytes;

        public WhenUploadingArticleImageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var article = ArticleBuilder.WithLibrary(LibraryId).Build();
            _articleId = article.Id;

            var imageUrl = ArticleTestRepository.GetArticleImageUrl(_articleId);

            _response = await Client.PutFile($"/libraries/{LibraryId}/articles/{_articleId}/image", _newImage);
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedArticleImage()
        {
            _assert.ShouldHaveUpdatedArticleImage(_articleId, _newImage);
        }
    }
}
