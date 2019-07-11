using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.DeleteBook
{
    [TestFixture]
    public class WhenDeletingBookAsWriter : FunctionTest
    {
        NoContentResult _response;

        private Ports.Database.Entities.Library.Book _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var books = _dataBuilder.WithBooks(1, true, 1).Build();
            _expected = books.First();
            
            var handler = Container.GetService<Functions.Library.Books.DeleteBook>();
            _response = (NoContentResult) await handler.Run(request, NullLogger.Instance, _expected.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedBook()
        {
            var cat = _dataBuilder.GetById(_expected.Id);
            Assert.That(cat, Is.Null, "Book should be deleted.");
        }

        [Test]
        public void ShouldHaveDeletedTheBookImage()
        {
            var db = Container.GetService<IDatabaseContext>();
            var file = db.File.SingleOrDefault(i => i.Id == _expected.ImageId);
            Assert.That(file, Is.Null, "Book Image should be deleted");
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            var db = Container.GetService<IDatabaseContext>();
            var author = db.Author.SingleOrDefault(a => a.Id == _expected.AuthorId);
            Assert.That(author, Is.Not.Null, "Book author should not be deleted");
        }

        [Test]
        public void ShouldNotHaveDeletedTheSeries()
        {
            var db = Container.GetService<IDatabaseContext>();
            var series = db.Series.SingleOrDefault(a => a.Id == _expected.SeriesId);
            Assert.That(series, Is.Not.Null, "Book series should not be deleted");
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var db = Container.GetService<IDatabaseContext>();
            var category = db.Category.SingleOrDefault(a => a.Id == _expected.BookCategory.First().CategoryId);
            Assert.That(category, Is.Not.Null, "Book category should not be deleted");
        }
    }
}
