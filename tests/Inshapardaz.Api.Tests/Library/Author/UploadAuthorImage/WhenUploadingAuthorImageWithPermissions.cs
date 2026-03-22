using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUploadingAuthorImageWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private AuthorAssert _assert;
        private int _authorId;
        private byte[] _newImage;

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
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveUpdatedAuthorImage() => _assert.ShouldHaveUpdatedAuthorImage(_authorId, _newImage);
    }
}
