using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.DeleteBookFile
{
    [TestFixture, Ignore("ToFix")]
    public class WhenDeletingBookFileAsUnauthorised
        : LibraryTest<Functions.Library.Books.Files.DeleteBookFile>
    {
        private UnauthorizedResult _response;

        private BookFileDto _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = _dataBuilder.WithFile().Build();
            _expected = _dataBuilder.Files.PickRandom();

            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, _expected.BookId, _expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
            var files = DatabaseConnection.GetBookFiles(_expected.BookId);
            Assert.That(files, Is.Not.Empty, "Book File should not be deleted.");
        }

        [Test]
        public void ShouldNotHaveDeletedFile()
        {
            var file = DatabaseConnection.GetFileById(_expected.FileId);
            Assert.That(file, Is.Not.Null, "File should not be deleted.");
        }
    }
}
