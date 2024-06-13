using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.DeleteBook
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private BookDto _expected;
        private string _imageFilePath;
        private int _authorId;

        public WhenDeletingBookWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .Build(1);

            _authorId = BookBuilder.Authors.First().Id;
            _expected = books.PickRandom();

            
            _imageFilePath = DatabaseConnection.GetFileById(_expected.ImageId.Value).FilePath;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedBook()
        {
            BookAssert.ShouldHaveDeletedBook(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheBookImage()
        {
            BookAssert.ShouldHaveDeletedBookImage(_expected.Id, _expected.ImageId, _imageFilePath, DatabaseConnection, FileStore);
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            AuthorAssert.ShouldNotHaveDeletedAuthor(_authorId, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheSeries()
        {
            SeriesAssert.ShouldNotHaveDeletedSeries(_expected.SeriesId.Value, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = DatabaseConnection.GetCategoriesByBook(_expected.Id);
            cats.ForEach(cat => CategoryAssert.ShouldNotHaveDeletedCategory(LibraryId, cat.Id, DatabaseConnection));
        }

        [Test]
        public void ShouldNotHaveDeletedAllChapters()
        {
            var chapters = BookBuilder.Chapters.Where(c => c.BookId == _expected.Id).ToList();

            foreach (var chapter in chapters)
            {
                ChapterAssert.ShouldHaveDeletedChapter(chapter.Id, DatabaseConnection);
            }
        }

        [Test]
        public void ShouldNotHaveDeletedAllContents()
        {
            var contents = BookBuilder.Contents.Where(c => c.BookId == _expected.Id).ToList();

            foreach (var content in contents)
            {
                FileAssert.FileDoesnotExist(content.Id, content.FilePath);
            }
        }

        [Test]
        public void ShouldBeDeletedFromTheFavoritesOfAllUsers()
        {
            BookAssert.ShouldNotBeInFavorites(_expected.Id, AccountId, DatabaseConnection);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadBooks()
        {
            BookAssert.ShouldHaveDeletedBookFromRecentReads(_expected.Id, DatabaseConnection);
        }
    }
}
