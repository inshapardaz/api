using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.GetBookContent
{
    [TestFixture]
    public class WhenGettingBookContentForBookWithNoContents
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private NotFoundResult _response;

        private BookDto _book;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.WithLibrary(LibraryId).Build();
            var request = new RequestBuilder().WithAccept(MimeTypes.Pdf).WithLanguage(Random.Locale).Build();
            _response = (NotFoundResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeNotFound();
        }
    }
}
