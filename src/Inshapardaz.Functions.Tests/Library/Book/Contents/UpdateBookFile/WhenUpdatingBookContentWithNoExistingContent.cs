using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.UpdateBookFile
{
    [TestFixture]
    public class WhenUpdatingBookContentWithNoExistingContent
        : LibraryTest<Functions.Library.Books.Content.UpdateBookContent>
    {
        private CreatedResult _response;
        private BookContentAssert _assert;
        private BookDto _book;
        private string _mimeType;
        private string _locale;
        private byte[] _contents;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _mimeType = Random.MimeType;
            _locale = Random.Locale;

            _book = _dataBuilder.WithLibrary(LibraryId).Build();
            _contents = new Faker().Image.Random.Bytes(50);
            var request = new RequestBuilder().WithBytes(_contents)
                                              .WithContentType(_mimeType)
                                              .WithLanguage(_locale)
                                              .BuildRequestMessage();
            _response = (CreatedResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

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
