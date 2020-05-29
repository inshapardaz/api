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
    public class WhenDeletingBookFileAsWriter
        : LibraryTest<Functions.Library.Books.Content.DeleteBookContent>
    {
        private NoContentResult _response;

        private BookFileDto _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = _dataBuilder.WithContent().Build();
            _expected = _dataBuilder.Files.PickRandom();

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.BookId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedBookFile()
        {
            var files = DatabaseConnection.GetBookContents(_expected.BookId);
            Assert.That(files, Is.Empty, "Book File should be deleted.");
        }

        [Test]
        public void ShouldHaveDeletedFile()
        {
            var file = DatabaseConnection.GetFileById(_expected.FileId);
            Assert.That(file, Is.Null, "File should be deleted.");
        }
    }
}
