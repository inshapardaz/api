using System;
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
    public class WhenDeletingBookAsAdministrator : LibraryTest<Functions.Library.Books.DeleteBook>
    {
        private NoContentResult _response;

        private BookDto _expected;
        private BooksDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<BooksDataBuilder>();
            var books = _dataBuilder.WithCategories(1).HavingSeries().Build(1);
            _expected = books.First();

            DatabaseConnection.AddBookToFavorites(_expected.Id, Guid.NewGuid());
            DatabaseConnection.AddBookToRecentReads(_expected.Id, Guid.NewGuid());

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            Check.ThatFileIsDeleted(_expected.ImageId.Value);
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            Check.ThatAuthorExists(_expected.AuthorId);
        }

        [Test]
        public void ShouldNotHaveDeletedTheSeries()
        {
            Check.ThatSeriesExists(_expected.SeriesId.Value);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = DatabaseConnection.GetCategoriesByBook(_expected.Id);
            cats.ForEach(cat => Check.ThatCategoryExists(cat.Id));
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
