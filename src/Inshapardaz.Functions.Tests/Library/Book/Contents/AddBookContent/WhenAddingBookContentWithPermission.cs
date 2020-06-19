using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.AddBookContent
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenAddingBookContentWithPermission
        : LibraryTest<Functions.Library.Books.Content.AddBookContent>
    {
        private CreatedResult _response;
        private string _mimeType;
        private string _locale;
        private BookContentAssert _assert;
        private ClaimsPrincipal _claim;
        private byte[] _contents;

        public WhenAddingBookContentWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _mimeType = Random.MimeType;
            _locale = Random.Locale;

            var dataBuilder = Container.GetService<BooksDataBuilder>();

            var book = dataBuilder.WithLibrary(LibraryId).Build();
            _contents = Random.Bytes;

            var request = new RequestBuilder().WithBytes(_contents).WithContentType(_mimeType).WithLanguage(_locale).BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, book.Id, _claim, CancellationToken.None);

            _assert = new BookContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveCorrectLink()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(_locale);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_mimeType);
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveBookContent(_contents, DatabaseConnection, FileStorage);
        }
    }
}
