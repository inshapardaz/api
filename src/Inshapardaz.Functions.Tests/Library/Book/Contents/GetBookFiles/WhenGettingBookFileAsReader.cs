using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Contents.GetBookFile
{
    [TestFixture, Ignore("ToFix")]
    public class WhenGettingBookFileAsReader
        : LibraryTest<Functions.Library.Books.Content.GetBookContent>
    {
        private OkObjectResult _response;

        private BookDto _book;
        private BookContentView _view;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.WithContents(5).Build();
            var request = new RequestBuilder().Build();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _view = (BookContentView)_response.Value;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldReturnAllBookFiles()
        {
            foreach (var bookFile in _dataBuilder.Contents)
            {
                //var file = DatabaseConnection.GetFileById(bookFile.FileId);

                //var actual = _view.Items.SingleOrDefault(f => f.Id == file.Id);
                //Assert.That(actual, Is.Not.Null, "File ot found in resonse");
                //Assert.That(actual.FileName, Is.EqualTo(file.FileName), "File Name doesn't match");
                //Assert.That(actual.MimeType, Is.EqualTo(file.MimeType), "MimeType doesn't match");
            }
        }
    }
}
