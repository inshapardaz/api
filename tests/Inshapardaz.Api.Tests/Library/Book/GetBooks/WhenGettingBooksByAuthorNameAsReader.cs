using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksByAuthorNameAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private AuthorDto _author;
        private IEnumerable<BookDto> _authorBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorBooks = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).IsPublic().Build(3);
            BookBuilder.WithLibrary(LibraryId).IsPublic().Build(10);

            var namePart = _author.Name.Substring(0, _author.Name.Length / 2);
            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?authorName={Uri.EscapeDataString(namePart)}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyBooksByMatchingAuthor()
        {
            _assert.ShouldHaveTotalCount(_authorBooks.Count())
                   .ShouldHaveItems(_authorBooks.Count());
        }
    }
}
