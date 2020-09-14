﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageWhenNonExistingAuthor : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUploadingAuthorImageWhenNonExistingAuthor()
            : base(Permission.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newimage = Random.Bytes;
            var client = CreateClient();
            _response = await client.PutFile($"/library/{LibraryId}/authors/{-Random.Number}/image", newimage);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
