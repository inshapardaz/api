using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
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

            var imageUrl = DatabaseConnection.GetArticleImageUrl(_articleId);

            _response = await Client.PutFile($"/libraries/{LibraryId}/articles/{_articleId}/image", _newImage);
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
            ArticleAssert.ShouldHaveUpdatedArticleImage(_articleId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
