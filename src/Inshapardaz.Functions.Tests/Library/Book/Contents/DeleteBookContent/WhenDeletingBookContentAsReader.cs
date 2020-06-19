using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.DeleteBookContent
{
    [TestFixture]
    public class WhenDeletingBookContentAsReader
        : LibraryTest<Functions.Library.Books.Content.DeleteBookContent>
    {
        private ForbidResult _response;

        private BookContentDto _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = _dataBuilder.WithLibrary(LibraryId).WithContent().Build();
            _expected = _dataBuilder.Contents.PickRandom();

            var request = new RequestBuilder().WithLanguage(_expected.Language).WithAccept(_expected.MimeType).Build();
            _response = (ForbidResult)await handler.Run(request, LibraryId, book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnForbidden()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotDeletedBookFile()
        {
            BookContentAssert.ShouldHaveBookContent(_expected.BookId, _expected.Language, _expected.MimeType, DatabaseConnection);
        }
    }
}