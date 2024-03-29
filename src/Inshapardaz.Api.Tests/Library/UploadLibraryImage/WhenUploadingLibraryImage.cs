﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.UploadLibraryImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenUploadingLibraryImage : TestBase
    {
        private HttpResponseMessage _response;
        private byte[] _newImage;

        public WhenUploadingLibraryImage(Role Role)
            : base(Role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newImage = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/image", _newImage);
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
        public void ShouldHaveUpdatedImage()
        {
            LibraryAssert.ShouldHaveUpdatedImage(LibraryId, _newImage, DatabaseConnection, FileStore);
        }
    }
}
