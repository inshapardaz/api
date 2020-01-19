using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.Files.GetBookFile
{
    [TestFixture]
    public class WhenGettingBookFileForBookWithNoFiles : FunctionTest
    {
        private OkObjectResult _response;

        private Ports.Database.Entities.Library.Book _book;
        private BookFilesView _view;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<BooksDataBuilder>();

            _book = _dataBuilder.Build();
            var request = new RequestBuilder().Build();
            var handler = Container.GetService<Functions.Library.Books.Files.GetBookFiles>();
            _response = (OkObjectResult)await handler.Run(request, _book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

            _view = (BookFilesView)_response.Value;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldReturnNoFiles()
        {
            Assert.That(_view.Items, Is.Empty);
        }
    }
}
