using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UploadAuthorImage
{
    [TestFixture]
    public class WhenUploadingAuthorImageAsAdministrator : FunctionTest
    {
        OkResult _response;
        private AuthorsDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _authorId;
        private byte[] _oldImage;
        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;
            
            var author = _builder.WithAuthors(1).Build().Single();
            _authorId = author.Id;
            var imageUrl = _builder.GetAuthorImageUrl(_authorId);
            _oldImage = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            var handler = Container.GetService<Functions.Library.Authors.UpdateAuthorImage>();
            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (OkResult) await handler.Run(request, _authorId, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }


        [Test]
        public async Task ShouldHaveUpdatedAuthorImage()
        {
            var imageUrl = _builder.GetAuthorImageUrl(_authorId);
            Assert.That(imageUrl, Is.Not.Null, "Author should have an image url`.");
            var image = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            Assert.That(image, Is.Not.Null, "Author should have an image.");
            Assert.That(image, Is.Not.EqualTo(_oldImage), "Author image should have updated.");
        }
    }
}
