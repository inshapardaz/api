using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UploadBookImage
{
    [TestFixture]
    public class WhenUploadingBookImageWhenNoExistingImage : LibraryTest<Functions.Library.Books.UpdateBookImage>
    {
        private CreatedResult _response;
        private BookAssert _bookAssert;
        private BooksDataBuilder _builder;
        private FakeFileStorage _fileStorage;
        private int _bookId;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();
            _fileStorage = Container.GetService<IFileStorage>() as FakeFileStorage;

            var book = _builder.WithLibrary(LibraryId).WithNoImage().Build();
            _bookId = book.Id;

            var request = new RequestBuilder().WithImage().BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
            _bookAssert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _bookAssert.ShouldHaveCorrectImageLocationHeader(_bookId);
        }

        [Test]
        public void ShouldHaveAddedImageToBook()
        {
            BookAssert.ShouldHaveAddedBookImage(_bookId, DatabaseConnection, _fileStorage);
        }

        [Test]
        public void ShouldHavePublicImage()
        {
            BookAssert.ShouldHavePublicImage(_bookId, DatabaseConnection);
        }
    }
}
