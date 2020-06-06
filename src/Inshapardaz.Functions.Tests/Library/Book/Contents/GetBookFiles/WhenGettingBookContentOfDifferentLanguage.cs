using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.GetBookFile
{
    [TestFixture]
    public class WhenGettingBookContentOfDifferentLanguage
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private StatusCodeResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _dataBuilder = Container.GetService<BooksDataBuilder>();

            var _book = _dataBuilder.WithLibrary(LibraryId).WithContents(5).Build();
            var _expected = _dataBuilder.Contents.PickRandom();

            var request = new RequestBuilder().WithAccept(_expected.MimeType).WithLanguage(_expected.Language + "a").Build();
            _response = (StatusCodeResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
