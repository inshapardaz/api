using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.BookCategories.SetBookCategories
{
    public class WhenSettingBookCategoriesAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private BookDto _book;
        private List<CategoryDto> _existing;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _existing = CategoryBuilder.WithLibrary(LibraryId).Build(2).ToList();
            var other = CategoryBuilder.WithLibrary(LibraryId).Build();
            _book = BookBuilder.WithLibrary(LibraryId).IsPublic().WithCategories(_existing).Build();

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_book.Id}/categories", new List<int> { other.Id });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveChangedCategories()
        {
            CategoryTestRepository.GetCategoriesByBook(_book.Id).Select(c => c.Id)
                .Should().BeEquivalentTo(_existing.Select(c => c.Id));
        }
    }
}
