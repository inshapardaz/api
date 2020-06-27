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
    public class WhenUpdatingBookContentWithDifferentLanguage
        : LibraryTest<Functions.Library.Books.Content.UpdateBookContent>
    {
        private CreatedResult _response;
        private string _newLanguage;
        private BookDto _book;
        private BookContentDto _file;
        private byte[] _contents;
        private BooksDataBuilder _dataBuilder;
        private BookContentAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newLanguage = Random.Locale;
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.WithLibrary(LibraryId).WithContents(2).WithContentLanguage($"{_newLanguage}_old").Build();
            _file = _dataBuilder.Contents.PickRandom();

            _contents = new Faker().Image.Random.Bytes(50);

            var request = new RequestBuilder()
                .WithBytes(_contents)
                .WithContentType(_file.MimeType)
                .WithLanguage(_newLanguage)
                .BuildRequestMessage();

            _response = (CreatedResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

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
            _assert.ShouldHaveCorrectLanguage(_newLanguage);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_file.MimeType);
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveBookContent(_contents, DatabaseConnection, FileStorage);
        }
    }
}
