using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.DeleteBookFile
{
    [TestFixture]
    public class WhenDeletingBookFileAsUnauthorised : FunctionTest
    {
        private UnauthorizedResult _response;

        private Ports.Database.Entities.Library.BookFile _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = _dataBuilder.WithFile().Build();
            _expected = book.Files.First();
            
            var handler = Container.GetService<Functions.Library.Books.Files.DeleteBookFile>();
            _response = (UnauthorizedResult)await handler.Run(request, _expected.BookId, _expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Assert.That(_response, Is.Not.Null);
        }

        [Test]
        public void ShouldNotDeletedBookFile()
        {
            var files = _dataBuilder.GetBookFiles(_expected.BookId);
            Assert.That(files, Is.Not.Empty, "Book File should not be deleted.");
        }

        [Test]
        public void ShouldNotHaveDeletedFile()
        {
            var file = _dataBuilder.GetFileById(_expected.FileId);
            Assert.That(file, Is.Not.Null, "File should not be deleted.");
        }
    }
}
