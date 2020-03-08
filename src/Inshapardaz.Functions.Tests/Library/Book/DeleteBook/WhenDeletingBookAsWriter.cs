using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.DeleteBook
{
    [TestFixture]
    public class WhenDeletingBookAsWriter : LibraryTest<Functions.Library.Books.DeleteBook>
    {
        private NoContentResult _response;

        private BookDto _expected;
        private BooksDataBuilder _dataBuilder;
        private IEnumerable<CategoryDto> _categories;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var books = _dataBuilder.HavingSeries().WithCategories(4).Build(1);
            _categories = DatabaseConnection.GetCategoriesByBook(_expected.Id);

            _expected = books.First();

            _dataBuilder.AddBookToFavorite(_expected, Guid.NewGuid());
            DatabaseConnection.AddBookToRecentReads(_expected.Id, Guid.NewGuid());

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedBook()
        {
            var cat = DatabaseConnection.GetBookById(_expected.Id);
            Assert.That(cat, Is.Null, "Book should be deleted.");
        }

        [Test]
        public void ShouldHaveDeletedTheBookImage()
        {
            var file = DatabaseConnection.GetFileById(_expected.ImageId.Value);
            Assert.That(file, Is.Null, "Book Image should be deleted");
        }

        [Test]
        public void ShouldHaveDeletedTheBookChapters()
        {
            var chapters = DatabaseConnection.GetChaptersByBook(_expected.Id);
            Assert.That(chapters.Any(i => i.BookId == _expected.Id), Is.False, "Book chapters should be deleted");
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            var author = DatabaseConnection.GetAuthorById(_expected.AuthorId);
            Assert.That(author, Is.Not.Null, "Book author should not be deleted");
        }

        [Test]
        public void ShouldNotHaveDeletedTheSeries()
        {
            var series = DatabaseConnection.GetSeriesById(_expected.SeriesId.Value);
            Assert.That(series, Is.Not.Null, "Book series should not be deleted");
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            foreach (var cat in _categories)
            {
                Assert.That(DatabaseConnection.DoesCategoryExists(cat.Id), Is.Not.Null, "Book category should not be deleted");
            }
        }

        [Test]
        public void ShouldBeDeletedFromTheFavoritesOfAllUsers()
        {
            Assert.That(DatabaseConnection.DoesBookExistsInFavorites(_expected.Id), Is.False);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadBooks()
        {
            Assert.That(DatabaseConnection.DoesBookExistsInRecent(_expected.Id), Is.False);
        }
    }
}
