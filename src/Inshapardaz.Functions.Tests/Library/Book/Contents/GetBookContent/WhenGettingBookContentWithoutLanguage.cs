using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class WhenGettingBookContentWithoutLanguage
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private OkObjectResult _response;
        private BookContentAssert _assert;
        private BookDto _book;
        private BookContentDto _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.WithLibrary(LibraryId).WithContents(1).WithContentLanguage(Library.Language).Build();
            _expected = _dataBuilder.Contents.PickRandom();

            var request = new RequestBuilder().WithAccept(_expected.MimeType).WithoutLanguage().Build();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _assert = new BookContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
            _assert.ShouldHaveCorrectLanguage(Library.Language);
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink();
            _assert.ShouldNotHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_expected.MimeType);
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(Library.Language);
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            _assert.ShouldMatch(_expected, _book.Id, DatabaseConnection);
        }
    }
}
