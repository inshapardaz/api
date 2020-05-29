using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.GetBookFile
{
    [TestFixture, Ignore("ToFix")]
    public class WhenGettingBookFileAsUnauthorised
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
            _response = (OkObjectResult)await handler.Run(request, LibraryId, _book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);

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
            //_view.Items.ForEach(f => Check.ThatFileMatch(f));
        }
    }
}
