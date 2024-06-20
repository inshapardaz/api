using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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

        public WhenSearchingBooksByTitleAndAuthor() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _authorBooks = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).IsPublic().Build(25);
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=2&pageSize=10&authorId={_author.Id}&query=itle");
            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", new KeyValuePair<string, string>("query", "itle"));
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books", new KeyValuePair<string, string>("authorid", _author.Id.ToString()));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3, 10, new KeyValuePair<string, string>("query", "itle"));
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3, 10, new KeyValuePair<string, string>("authorid", _author.Id.ToString()));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1, 10, new KeyValuePair<string, string>("query", "itle"));
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1, 10, new KeyValuePair<string, string>("authorid", _author.Id.ToString()));
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _authorBooks.Where(b => b.Title.Contains("itle"))
                                            .OrderBy(a => a.Title).Skip(10).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<BookAssert>().ForView(actual).ForLibrary(LibraryId)
                            .ShouldBeSameAs(expected)
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
