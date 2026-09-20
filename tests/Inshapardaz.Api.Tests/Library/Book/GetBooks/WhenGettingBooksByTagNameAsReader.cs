using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    // Regression test for the MySQL Tag join bug (issue #62): the Tag join was
    // `ON bt.TagId = c.Id` (Category's Id, not Tag's), so t.Name never matched a real tag row.
    [TestFixture]
    public class WhenGettingBooksByTagNameAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private TagDto _tag;
        private IEnumerable<BookDto> _tagBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _tag = TagBuilder.WithLibrary(LibraryId).Build();
            _tagBooks = BookBuilder.WithLibrary(LibraryId).WithTag(_tag).IsPublic().Build(3);
            BookBuilder.WithLibrary(LibraryId).IsPublic().Build(10);

            var namePart = _tag.Name.Substring(0, _tag.Name.Length / 2);
            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?tagName={Uri.EscapeDataString(namePart)}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyBooksWithMatchingTag()
        {
            _assert.ShouldHaveTotalCount(_tagBooks.Count())
                   .ShouldHaveItems(_tagBooks.Count());
        }
    }
}
