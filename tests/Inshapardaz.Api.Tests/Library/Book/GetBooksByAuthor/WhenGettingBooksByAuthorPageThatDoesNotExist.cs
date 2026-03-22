using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooksByAuthor
{
    [TestFixture]
    public class WhenGettingBooksByAuthorPageThatDoesNotExist() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private AuthorDto _author;
        private IEnumerable<BookDto> _authorBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorBooks = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).IsPublic().Build(1);
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=20&pageSize=10&authorId={_author.Id}");
            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSelfLink() => _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books");

        [Test]
        public void ShouldNotHaveNextLink() => _assert.ShouldNotHaveNextLink();

        [Test]
        public void ShouldNotHavePreviousLink() => _assert.ShouldNotHavePreviousLink();

        [Test]
        public void ShouldNotHaveCreateLink() => _assert.ShouldNotHaveCreateLink();

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(20)
                   .ShouldHavePageSize(10)
                   .ShouldHaveTotalCount(_authorBooks.Count());
        }

        [Test]
        public void ShouldReturnNoBooks() => _assert.ShouldHaveNoData();
    }
}
