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

namespace Inshapardaz.Functions.Tests.Library.Book.UploadBookImage
{
    [TestFixture]
    public class WhenUploadingBookImageAsReader : FunctionTest
    {
        ForbidResult _response;
        private BooksDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _bookId;
        private byte[] _oldImage;
        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;
            
            var book = _builder.WithBooks(1).Build().Single();
            _bookId = book.Id;
            var imageUrl = _builder.GetBookImageUrl(_bookId);
            _oldImage = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            var handler = Container.GetService<Functions.Library.Books.UpdateBookImage>();
            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (ForbidResult) await handler.Run(request, _bookId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            Assert.That(_response, Is.Not.Null);
        }


        [Test]
        public async Task ShouldHaveUpdatedBookImage()
        {
            var imageUrl = _builder.GetBookImageUrl(_bookId);
            Assert.That(imageUrl, Is.Not.Null, "Book should have an image url`.");
            var image = await _fileStorage.GetFile(imageUrl, CancellationToken.None);
            Assert.That(image, Is.Not.Null, "Book should have an image.");
            Assert.That(image, Is.EqualTo(_oldImage), "Book image should not be updated.");
        }
    }
}
