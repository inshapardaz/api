using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    // authorName/tagName/query (and every other supplied filter) combine with AND.
    [TestFixture]
    public class WhenGettingBooksByCombinedNameFilters() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private AuthorDto _author;
        private TagDto _tag;
        private BookDto _matchingBook;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorBuilder.WithLibrary(LibraryId).Build();
            _tag = TagBuilder.WithLibrary(LibraryId).Build();

            // Shares the author but not the tag -- must be excluded by the AND.
            BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).IsPublic().Build(2);
            // Shares the tag but not the author -- must be excluded by the AND.
            BookBuilder.WithLibrary(LibraryId).WithTag(_tag).IsPublic().Build(2);

            _matchingBook = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).WithTag(_tag).IsPublic().Build();

            var authorNamePart = _author.Name.Substring(0, _author.Name.Length / 2);
            var tagNamePart = _tag.Name.Substring(0, _tag.Name.Length / 2);
            _response = await Client.GetAsync(
                $"/libraries/{LibraryId}/books?authorName={Uri.EscapeDataString(authorNamePart)}&tagName={Uri.EscapeDataString(tagNamePart)}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyTheBookMatchingBothFilters()
        {
            _assert.ShouldHaveTotalCount(1)
                   .ShouldHaveItems(1);

            _assert.Data.Single().Id.Should().Be(_matchingBook.Id);
        }
    }
}
