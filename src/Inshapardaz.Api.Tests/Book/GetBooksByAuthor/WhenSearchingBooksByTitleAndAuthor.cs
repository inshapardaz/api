using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksByAuthor
{
    [TestFixture]
    public class WhenSearchingBooksByTitleAndAuthor
        : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private AuthorDto _author;
        private IEnumerable<BookDto> _authorBooks;

        public WhenSearchingBooksByTitleAndAuthor() : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorBooks = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).IsPublic().Build(25);
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/library/{LibraryId}/books?pageNumber=2&pageSize=10&authorId={_author.Id}&query=itle");
            _assert = new PagingAssert<BookView>(_response, Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books", "query", "itle");
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/books", "authorid", _author.Id.ToString());
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/books", 3, 10, "query", "itle");
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/books", 3, 10, "authorid", _author.Id.ToString());
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books", 1, 10, "query", "itle");
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/books", 1, 10, "authorid", _author.Id.ToString());
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _authorBooks.Where(b => b.Title.Contains("itle"))
                                            .OrderBy(a => a.Title).Skip(10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item, DatabaseConnection)
                            .InLibrary(LibraryId)
                            .ShouldHaveCorrectLinks()
                            .ShouldNotHaveEditLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveCreateChaptersLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveChaptersLink()
                            .ShouldHavePublicImageLink();
            }
        }
    }
}
