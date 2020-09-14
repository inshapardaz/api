﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UploadAuthorImage
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenUploadingAuthorImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private int _authorId;
        private byte[] _newImage;

        public WhenUploadingAuthorImageWithPermissions(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorId = author.Id;

            _newImage = Random.Bytes;

            _response = await Client.PutFile($"/library/{LibraryId}/authors/{_authorId}/image", _newImage);
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
        public void ShouldHaveUpdatedAuthorImage()
        {
            AuthorAssert.ShouldHaveUpdatedAuthorImage(_authorId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
