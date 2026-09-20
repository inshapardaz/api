using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksByLanguageAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private IEnumerable<BookDto> _urduBooks;

        [OneTimeSetUp]
        public async Task Setup()
        {
            BookBuilder.WithLibrary(LibraryId).WithLanguage("en").IsPublic().Build(10);
            _urduBooks = BookBuilder.WithLibrary(LibraryId).WithLanguage("ur").IsPublic().Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?language=ur");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyBooksInThatLanguage()
        {
            _assert.ShouldHaveTotalCount(_urduBooks.Count())
                   .ShouldHaveItems(_urduBooks.Count());

            foreach (var actual in _assert.Data)
            {
                actual.Language.Should().Be("ur");
            }
        }
    }
}
