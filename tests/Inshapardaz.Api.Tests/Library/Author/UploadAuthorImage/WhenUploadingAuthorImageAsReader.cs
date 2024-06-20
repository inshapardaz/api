using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private AuthorAssert _assert;
        private int _authorId;
        private byte[] _newImage;

        public WhenUploadingAuthorImageAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorId = author.Id;
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/authors/{_authorId}/image", _newImage);
            _assert = Services.GetService<AuthorAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedAuthorImage()
        {
            _assert.ShouldNotHaveUpdatedAuthorImage(_authorId, _newImage);
        }
    }
}
