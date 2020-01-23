using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksWithQuery : FunctionTest
    {
        private OkObjectResult _response;
        private PageView<BookView> _view;
        private Ports.Database.Entities.Library.Book _searchedBook;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = Container.GetService<BooksDataBuilder>();
            var books = builder.Build(20);
            _searchedBook = books.Last();

            var request = new RequestBuilder()
                          .WithQueryParameter("query", _searchedBook.Title)
                          .Build();

            var handler = Container.GetService<Functions.Library.Books.GetBooks>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as PageView<BookView>;
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
        public void ShouldHaveFoundSearchedBook()
        {
            var actual = _view.Data.FirstOrDefault();
            Assert.That(actual, Is.Not.Null, "Should have found a book");
            Assert.That(actual.Id, Is.EqualTo(_searchedBook.Id), "Should have found correct book");
            Assert.That(actual.Title, Is.EqualTo(_searchedBook.Title), "Should have found correct book");
        }
    }
}
