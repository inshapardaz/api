using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.DeleteBook
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingBookWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private BookAssert _bookAssert;
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

            
            _imageFilePath = FileTestRepository.GetFileById(_expected.ImageId.Value).FilePath;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_expected.Id}");
            _bookAssert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _bookAssert.ShouldHaveDeletedBook(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheBookImage()
        {
            _bookAssert.ShouldHaveDeletedBookImage(_expected.Id, _expected.ImageId, _imageFilePath);
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            Services.GetService<AuthorAssert>()
                .ShouldNotHaveDeletedAuthor(_authorId);
        }

        [Test]
        public void ShouldNotHaveDeletedTheSeries()
        {
            Services.GetService<SeriesAssert>()
                .ShouldNotHaveDeletedSeries(_expected.SeriesId.Value);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = CategoryTestRepository.GetCategoriesByBook(_expected.Id);
            var catagoryAssert = Services.GetService<CategoryAssert>().ForLibrary(LibraryId);
            cats.ForEach(cat => catagoryAssert.ShouldNotHaveDeletedCategory(cat.Id));
        }

        [Test]
        public void ShouldNotHaveDeletedAllChapters()
        {
            var chapters = BookBuilder.Chapters.Where(c => c.BookId == _expected.Id).ToList();
            var chapterAssert = Services.GetService<ChapterAssert>().ForLibrary(LibraryId);

            foreach (var chapter in chapters)
            {
                chapterAssert.ShouldHaveDeletedChapter(chapter.Id);
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
            _bookAssert.ShouldNotBeInFavorites(_expected.Id, AccountId);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadBooks()
        {
            _bookAssert.ShouldHaveDeletedBookFromRecentReads(_expected.Id);
        }
    }
}
